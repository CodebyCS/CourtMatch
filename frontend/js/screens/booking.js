// ---------- BOOKING DETAIL (Gestão da reserva) + ADD EQUIPMENT ----------
async function openBookingDetail(id){
  S.booking = await api(API.booking, `/api/bookings/${id}`);
  S.bookingGames = S.booking.status === 'Confirmed' ? await getGamesForBooking(id) : [];
  go('booking');
}

async function openGame(id){
  S.game = S.bookingGames.find(g => g.id === id);
  S.sets = S.game.sets && S.game.sets.length
    ? S.game.sets.map(s => ({ setNumber:s.setNumber, teamOneGames:s.teamOneGames, teamTwoGames:s.teamTwoGames }))
    : [{ setNumber: 1, teamOneGames: 0, teamTwoGames: 0 }];
  await resolveUserNames(S.game.participants.map(p => p.userId));
  go('game');
}

function screenBookingDetail(){
  const b = S.booking;
  const court = S.courts.find(c => c.id === b.courtId);
  const equipRows = b.bookingEquipments && b.bookingEquipments.length ? b.bookingEquipments.map(be => {
    const e = S.equipments.find(x => x.id === be.equipmentId);
    return `<div class="equip-row"><div class="name">${e?e.name:'Equipamento'} ×${be.quantity}</div><div class="mono">${fmtMoney(be.totalPrice)}</div></div>`;
  }).join('') : '';

  const gamesList = S.bookingGames.length ? S.bookingGames.map(g => `
    <div class="row-item" onclick="openGame('${g.id}')">
      <div>
        <h3>${g.name || 'Jogo'}</h3>
        <div class="meta">${g.format === 'Singles' ? '1×1 Individual' : '2×2 Duplas'} · ${g.participants.length} jogador${g.participants.length===1?'':'es'}</div>
      </div>
      <span class="status-pill status-${g.status}">${g.status === 'Completed' && g.sets.length ? scoreLine(g) : g.status}</span>
    </div>
  `).join('') : `<div class="empty" style="padding:40px 20px;"><p class="mono" style="font-size:11px;text-transform:uppercase;letter-spacing:.05em;">Sem jogos<br><span style="font-size:10px;">Use os botões para adicionar atividades</span></p></div>`;

  return `
    ${backLink('mybookings','Voltar')}
    <div class="header-row">
      <div class="eyebrow" style="margin:0;flex:1;">Reserva ativa · ${new Date(b.startTime).toLocaleDateString('pt-PT',{day:'2-digit',month:'short',year:'numeric'})}</div>
      <span class="status-pill status-${b.status}">${b.status}</span>
    </div>
    <h2 style="font-size:26px;margin:0 0 22px;">Gestão da reserva</h2>

    <div class="split">
      <div class="col-a">
        <div class="section-label">Detalhes do campo</div>
        <h3 style="font-family:'Anton',sans-serif;text-transform:uppercase;font-size:20px;margin:0 0 4px;">${court ? court.name : 'Reserva'}</h3>
        ${court ? `<div class="meta" style="margin-bottom:16px;">${court.isIndoor?'Interior · Climatizado':'Exterior · Descoberto'}</div>` : ''}

        <div class="summary-row"><span>Data</span><span>${fmtDate(b.startTime)}</span></div>
        <div class="summary-row"><span>Até</span><span>${fmtDate(b.endTime)}</span></div>
        <div class="summary-row"><span>Preço do campo</span><span>${fmtMoney(b.courtPrice)}</span></div>
        ${equipRows}
        <div class="summary-row total"><span>Total pago</span><span>${fmtMoney(b.totalPrice)}</span></div>

        <div style="width:140px;color:var(--text-dim);margin:16px 0;">${courtDiagram()}</div>

        <div class="section-label">Adicionar à reserva</div>
        ${b.status === 'Pending' ? `
          <button class="btn btn-primary" onclick="bookingAction('confirm')">Confirmar reserva</button>
          <div style="height:10px;"></div>
          <button class="btn btn-danger" onclick="bookingAction('cancel')">Cancelar reserva</button>
        ` : ''}
        ${b.status === 'Confirmed' ? `
          <div class="tile" onclick="openNewGame()">
            <div class="icon-wrap">${icon('trophy')}</div>
            <div><div class="label">Criar jogo</div><div class="sub">Organizar partida</div></div>
          </div>
          <div class="tile" onclick="openEquipmentForBooking()">
            <div class="icon-wrap">${icon('racket')}</div>
            <div><div class="label">Equipamento</div><div class="sub">Raquetes, bolas, sapatilhas</div></div>
          </div>
        ` : ''}
      </div>

      <div class="col-b">
        <div class="section-label">Programação</div>
        ${gamesList}
      </div>
    </div>
  `;
}

async function bookingAction(action){
  try{
    S.booking = await api(API.booking, `/api/bookings/${S.booking.id}/${action}`, { method:'PATCH' });
    toast(action === 'confirm' ? 'Reserva confirmada!' : 'Reserva cancelada.', 'ok');
    render();
  } catch(e){}
}

function openEquipmentForBooking(){
  S.addEquipQty = {};
  go('addequip');
}

function addEquipTotal(){
  return Object.entries(S.addEquipQty).reduce((sum,[id,qty]) => {
    const e = S.equipments.find(x => x.id === id);
    return sum + (e ? e.rentalPrice * qty : 0);
  }, 0);
}

function stepAddEquip(id, delta){
  const cur = S.addEquipQty[id] || 0;
  const next = Math.max(0, cur + delta);
  if (next === 0) delete S.addEquipQty[id]; else S.addEquipQty[id] = next;
  render();
}

function screenAddEquipment(){
  const b = S.booking;
  const court = S.courts.find(c => c.id === b.courtId);
  const rows = S.equipments.length ? S.equipments.map(e => `
      <div class="equip-row">
        <div>
          <div class="name">${e.name}</div>
          <div class="sub">${fmtMoney(e.rentalPrice)} · stock ${e.stock}</div>
        </div>
        <div class="stepper">
          <button onclick="stepAddEquip('${e.id}',-1)">−</button>
          <span class="qty">${S.addEquipQty[e.id]||0}</span>
          <button onclick="stepAddEquip('${e.id}',1)">+</button>
        </div>
      </div>`).join('') : `<p class="mono" style="color:var(--text-dim);font-size:11.5px;">Nenhum equipamento registado.</p>`;

  return `
    ${backLink('booking','Voltar')}
    <div class="eyebrow">${court ? court.name : 'Reserva'} · ${fmtDate(b.startTime)}</div>
    <h2 style="font-size:26px;margin:0 0 22px;">Equipamento</h2>

    <div class="section" style="border-top:none;padding-top:0;">
      ${rows}
    </div>

    <div class="section">
      <div class="summary-row total"><span>Total a adicionar</span><span>${fmtMoney(addEquipTotal())}</span></div>
    </div>

    <button class="btn btn-primary" onclick="submitAddEquipment()">Adicionar à reserva</button>
  `;
}

async function submitAddEquipment(){
  const entries = Object.entries(S.addEquipQty);
  if (!entries.length){ toast('Escolha ao menos um item', 'error'); return; }
  try{
    for (const [equipId, qty] of entries){
      const e = S.equipments.find(x => x.id === equipId);
      await api(API.booking, `/api/bookings/${S.booking.id}/equipments`, { method:'POST', body:{
        equipmentId: equipId, quantity: qty, unitPrice: e.rentalPrice
      }});
    }
    toast('Equipamento adicionado!', 'ok');
    await openBookingDetail(S.booking.id);
  } catch(e){}
}
