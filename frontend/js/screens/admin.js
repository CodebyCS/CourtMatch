// ---------- ADMIN (Gestão) ----------
const EQUIP_CATEGORIES = ['Padel', 'Calçados', 'Vestuário', 'Acessórios'];
function equipCategoryOptions(){
  const set = new Set(EQUIP_CATEGORIES);
  S.equipments.forEach(e => { if (e.category) set.add(e.category); });
  return [...set];
}

function startEdit(type, id){ S.adminEdit = { type, id }; render(); }
function cancelEdit(){ S.adminEdit = { type: null, id: null }; render(); }

async function loadAdmin(){
  await loadCatalog();
  await resolveUserNames(S.allBookings.map(b => b.hostPlayerId));
}

function screenAdmin(){
  const tabs = [
    { k:'courts', l:'Campos' },
    { k:'equipments', l:'Equipamento' },
    { k:'timeslots', l:'Horários' },
    { k:'bookings', l:'Reservas' },
  ];

  return `
    <div class="eyebrow">Clube · Administração</div>
    <h2 style="font-size:24px;margin:0 0 22px;">Gestão</h2>

    <div class="tabs">
      ${tabs.map(t => `<div class="tab ${S.adminTab===t.k?'active':''}" onclick="S.adminTab='${t.k}';render()">${t.l}</div>`).join('')}
    </div>

    ${S.adminTab==='courts' ? adminCourtsTab() : ''}
    ${S.adminTab==='equipments' ? adminEquipmentsTab() : ''}
    ${S.adminTab==='timeslots' ? adminTimeSlotsTab() : ''}
    ${S.adminTab==='bookings' ? adminBookingsTab() : ''}
  `;
}

function adminCourtsTab(){
  const editing = S.adminEdit.type === 'court' ? S.courts.find(c => c.id === S.adminEdit.id) : null;

  const rows = S.courts.length ? S.courts.map(c => `
    <div class="row-item" style="cursor:default;">
      <div>
        <h3>${c.name}</h3>
        <span class="tag ${c.isIndoor?'tag-indoor':'tag-outdoor'}">${c.isIndoor?'Indoor':'Outdoor'}</span>
      </div>
      <div style="display:flex;align-items:center;gap:10px;">
        <div class="price-tag">${fmtMoney(c.pricePerHour)}<small>/ hora</small></div>
        <button class="btn btn-ghost btn-sm" onclick="startEdit('court','${c.id}')">Editar</button>
        <button class="btn btn-danger btn-sm" onclick="deleteCourtAdmin('${c.id}')">Apagar</button>
      </div>
    </div>`).join('') : emptyState('calendar', 'Nenhum campo registado ainda.');

  return `
    <div class="section" style="border-top:none;padding-top:0;">
      <div class="section-label">${editing ? `Editar campo — ${editing.name}` : 'Novo campo'}</div>
      <div class="field"><label>Nome</label><input id="af-court-name" type="text" placeholder="Campo 5" value="${editing?editing.name:''}"></div>
      <div class="chips" style="margin-bottom:16px;">
        <div class="chip ${!editing || !editing.isIndoor?'selected':''}" id="af-court-outdoor" onclick="toggleIndoorChip(false)">Outdoor</div>
        <div class="chip ${editing && editing.isIndoor?'selected':''}" id="af-court-indoor" onclick="toggleIndoorChip(true)">Indoor</div>
      </div>
      <div class="field"><label>Preço por hora (€)</label><input id="af-court-price" type="number" min="0" step="0.5" value="${editing?editing.pricePerHour:80}"></div>
      <div style="display:flex;gap:8px;">
        <button class="btn btn-primary" onclick="${editing?'updateCourtAdmin()':'createCourtAdmin()'}">${editing?'Guardar alterações':'Adicionar campo'}</button>
        ${editing ? `<button class="btn btn-ghost" onclick="cancelEdit()">Cancelar</button>` : ''}
      </div>
    </div>
    <div class="section">
      <div class="section-label">Campos existentes</div>
      ${rows}
    </div>
  `;
}

function toggleIndoorChip(indoor){
  document.getElementById('af-court-indoor').classList.toggle('selected', indoor);
  document.getElementById('af-court-outdoor').classList.toggle('selected', !indoor);
}

function readCourtForm(){
  return {
    name: document.getElementById('af-court-name').value.trim(),
    isIndoor: document.getElementById('af-court-indoor').classList.contains('selected'),
    pricePerHour: +document.getElementById('af-court-price').value,
  };
}

async function createCourtAdmin(){
  const f = readCourtForm();
  if (!f.name){ toast('Indica o nome do campo', 'error'); return; }
  try{
    await api(API.catalog, '/api/courts', { method:'POST', body:{ id: crypto.randomUUID(), ...f, status:'Available' } });
    toast('Campo criado!', 'ok');
    await loadAdmin();
    render();
  } catch(e){}
}

async function updateCourtAdmin(){
  const f = readCourtForm();
  if (!f.name){ toast('Indica o nome do campo', 'error'); return; }
  const original = S.courts.find(c => c.id === S.adminEdit.id);
  try{
    await api(API.catalog, '/api/courts', { method:'PUT', body:{ id: S.adminEdit.id, ...f, status: original ? original.status : 'Available' } });
    toast('Campo atualizado!', 'ok');
    cancelEdit();
    await loadAdmin();
    render();
  } catch(e){}
}

async function deleteCourtAdmin(id){
  try{
    await api(API.catalog, `/api/courts/${id}`, { method:'DELETE' });
    toast('Campo removido.', 'ok');
    await loadAdmin();
    render();
  } catch(e){}
}

function adminEquipmentsTab(){
  const editing = S.adminEdit.type === 'equipment' ? S.equipments.find(e => e.id === S.adminEdit.id) : null;
  const cats = equipCategoryOptions();
  const selectedCat = editing ? editing.category : cats[0];

  const rows = S.equipments.length ? S.equipments.map(e => `
    <div class="equip-row">
      <div>
        <div class="name">${e.name}</div>
        <div class="sub">${e.category} · stock ${e.stock}</div>
      </div>
      <div style="display:flex;align-items:center;gap:10px;">
        <div class="mono">${fmtMoney(e.rentalPrice)}</div>
        <button class="btn btn-ghost btn-sm" onclick="startEdit('equipment','${e.id}')">Editar</button>
        <button class="btn btn-danger btn-sm" onclick="deleteEquipmentAdmin('${e.id}')">Apagar</button>
      </div>
    </div>`).join('') : emptyState('calendar', 'Nenhum equipamento registado.');

  return `
    <div class="section" style="border-top:none;padding-top:0;">
      <div class="section-label">${editing ? `Editar equipamento — ${editing.name}` : 'Novo equipamento'}</div>
      <div class="field"><label>Nome</label><input id="af-eq-name" type="text" placeholder="Raquete Pro" value="${editing?editing.name:''}"></div>
      <div class="field">
        <label>Categoria</label>
        <select id="af-eq-cat">
          ${cats.map(c => `<option value="${c}" ${c===selectedCat?'selected':''}>${c}</option>`).join('')}
          <option value="__custom__">Outra (escrever)…</option>
        </select>
      </div>
      <div class="field" id="af-eq-cat-custom-wrap" style="display:none;"><label>Nova categoria</label><input id="af-eq-cat-custom" type="text" placeholder="Nome da categoria"></div>
      <div class="field"><label>Stock</label><input id="af-eq-stock" type="number" min="0" value="${editing?editing.stock:10}"></div>
      <div class="field"><label>Preço de aluguer (€)</label><input id="af-eq-price" type="number" min="0" step="0.5" value="${editing?editing.rentalPrice:15}"></div>
      <div style="display:flex;gap:8px;">
        <button class="btn btn-primary" onclick="${editing?'updateEquipmentAdmin()':'createEquipmentAdmin()'}">${editing?'Guardar alterações':'Adicionar equipamento'}</button>
        ${editing ? `<button class="btn btn-ghost" onclick="cancelEdit()">Cancelar</button>` : ''}
      </div>
    </div>
    <div class="section">
      <div class="section-label">Equipamento existente</div>
      ${rows}
    </div>
  `;
}

function readEquipmentForm(){
  const catSelect = document.getElementById('af-eq-cat').value;
  const category = catSelect === '__custom__' ? document.getElementById('af-eq-cat-custom').value.trim() : catSelect;
  return {
    name: document.getElementById('af-eq-name').value.trim(),
    category: category || 'Geral',
    stock: +document.getElementById('af-eq-stock').value,
    rentalPrice: +document.getElementById('af-eq-price').value,
  };
}

async function createEquipmentAdmin(){
  const f = readEquipmentForm();
  if (!f.name){ toast('Indica o nome do equipamento', 'error'); return; }
  try{
    await api(API.catalog, '/api/equipments', { method:'POST', body:{ id: crypto.randomUUID(), ...f } });
    toast('Equipamento criado!', 'ok');
    await loadAdmin();
    render();
  } catch(e){}
}

async function updateEquipmentAdmin(){
  const f = readEquipmentForm();
  if (!f.name){ toast('Indica o nome do equipamento', 'error'); return; }
  try{
    await api(API.catalog, '/api/equipments', { method:'PUT', body:{ id: S.adminEdit.id, ...f } });
    toast('Equipamento atualizado!', 'ok');
    cancelEdit();
    await loadAdmin();
    render();
  } catch(e){}
}

async function deleteEquipmentAdmin(id){
  try{
    await api(API.catalog, `/api/equipments/${id}`, { method:'DELETE' });
    toast('Equipamento removido.', 'ok');
    await loadAdmin();
    render();
  } catch(e){}
}

function adminTimeSlotsTab(){
  const editing = S.adminEdit.type === 'timeslot' ? S.timeslots.find(t => t.id === S.adminEdit.id) : null;

  const rows = S.timeslots.length ? S.timeslots.map(t => `
    <div class="row-item" style="cursor:default;">
      <div><h3>${t.name}</h3><div class="meta">${t.startTime.slice(0,5)} – ${t.endTime.slice(0,5)}</div></div>
      <div style="display:flex;align-items:center;gap:10px;">
        <button class="btn btn-ghost btn-sm" onclick="startEdit('timeslot','${t.id}')">Editar</button>
        <button class="btn btn-danger btn-sm" onclick="deleteTimeSlotAdmin('${t.id}')">Apagar</button>
      </div>
    </div>`).join('') : emptyState('calendar', 'Nenhum horário registado.');

  return `
    <div class="section" style="border-top:none;padding-top:0;">
      <div class="section-label">${editing ? `Editar horário — ${editing.name}` : 'Novo horário'}</div>
      <div class="field"><label>Nome</label><input id="af-slot-name" type="text" placeholder="Manhã" value="${editing?editing.name:''}"></div>
      <div class="field"><label>Início</label><input id="af-slot-start" type="time" value="${editing?editing.startTime.slice(0,5):'08:00'}"></div>
      <div class="field"><label>Fim</label><input id="af-slot-end" type="time" value="${editing?editing.endTime.slice(0,5):'09:30'}"></div>
      <div style="display:flex;gap:8px;">
        <button class="btn btn-primary" onclick="${editing?'updateTimeSlotAdmin()':'createTimeSlotAdmin()'}">${editing?'Guardar alterações':'Adicionar horário'}</button>
        ${editing ? `<button class="btn btn-ghost" onclick="cancelEdit()">Cancelar</button>` : ''}
      </div>
    </div>
    <div class="section">
      <div class="section-label">Horários existentes</div>
      ${rows}
    </div>
  `;
}

function readTimeSlotForm(){
  return {
    name: document.getElementById('af-slot-name').value.trim(),
    startTime: document.getElementById('af-slot-start').value + ':00',
    endTime: document.getElementById('af-slot-end').value + ':00',
  };
}

async function createTimeSlotAdmin(){
  const f = readTimeSlotForm();
  if (!f.name){ toast('Indica o nome do horário', 'error'); return; }
  try{
    await api(API.catalog, '/api/timeslots', { method:'POST', body:{ id: crypto.randomUUID(), ...f } });
    toast('Horário criado!', 'ok');
    await loadAdmin();
    render();
  } catch(e){}
}

async function updateTimeSlotAdmin(){
  const f = readTimeSlotForm();
  if (!f.name){ toast('Indica o nome do horário', 'error'); return; }
  try{
    await api(API.catalog, '/api/timeslots', { method:'PUT', body:{ id: S.adminEdit.id, ...f } });
    toast('Horário atualizado!', 'ok');
    cancelEdit();
    await loadAdmin();
    render();
  } catch(e){}
}

async function deleteTimeSlotAdmin(id){
  try{
    await api(API.catalog, `/api/timeslots/${id}`, { method:'DELETE' });
    toast('Horário removido.', 'ok');
    await loadAdmin();
    render();
  } catch(e){}
}

function adminBookingsTab(){
  const sorted = S.allBookings.slice().sort((a,b) => new Date(b.startTime) - new Date(a.startTime));
  const rows = sorted.length ? sorted.map(b => {
    const court = S.courts.find(c => c.id === b.courtId);
    return `
      <div class="row-item" style="cursor:default;">
        <div>
          <h3>${court ? court.name : 'Campo'}</h3>
          <div class="meta">${nameFor(b.hostPlayerId)} · ${fmtDate(b.startTime)}</div>
        </div>
        <div style="display:flex;align-items:center;gap:8px;">
          <span class="status-pill status-${b.status}">${b.status}</span>
          ${b.status==='Pending' ? `<button class="btn btn-ghost btn-sm" onclick="adminBookingAction('${b.id}','confirm')">Confirmar</button>` : ''}
          ${b.status==='Pending' ? `<button class="btn btn-danger btn-sm" onclick="adminBookingAction('${b.id}','cancel')">Cancelar</button>` : ''}
        </div>
      </div>`;
  }).join('') : emptyState('calendar', 'Nenhuma reserva no clube ainda.');

  return `<div class="section" style="border-top:none;padding-top:0;">${rows}</div>`;
}

async function adminBookingAction(id, action){
  try{
    await api(API.booking, `/api/bookings/${id}/${action}`, { method:'PATCH' });
    toast(action === 'confirm' ? 'Reserva confirmada!' : 'Reserva cancelada.', 'ok');
    await loadAdmin();
    render();
  } catch(e){}
}
