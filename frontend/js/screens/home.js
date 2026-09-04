// ---------- HOME ----------
async function loadCatalog(){
  const [courts, equipments, timeslots, allBookings] = await Promise.all([
    api(API.catalog, '/api/courts'),
    api(API.catalog, '/api/equipments'),
    api(API.catalog, '/api/timeslots'),
    api(API.booking, '/api/bookings?startDate=2020-01-01T00:00:00&endDate=2035-01-01T00:00:00'),
  ]);
  S.courts = courts || []; S.equipments = equipments || []; S.timeslots = timeslots || [];
  S.allBookings = (allBookings || []).filter(b => b.status !== 'Cancelled');
  if (!S.date) S.date = new Date().toISOString().slice(0,10);
}

function freeHoursFor(courtId, dateStr, duration){
  const dayBookings = S.allBookings.filter(b => b.courtId === courtId && b.startTime.slice(0,10) === dateStr);
  return HOURS.filter(h => {
    if (h + duration > 21) return false;
    const start = new Date(`${dateStr}T${fmtHour(h)}:00`);
    const end = new Date(start.getTime() + duration*3600*1000);
    return !dayBookings.some(b => {
      const bs = new Date(b.startTime), be = new Date(b.endTime);
      return start < be && end > bs;
    });
  });
}

function screenHome(){
  const dateObj = new Date(S.date + 'T00:00:00');
  const today = new Date().toISOString().slice(0,10);
  const tomorrow = new Date(Date.now() + 86400000).toISOString().slice(0,10);

  const hourChips = `<div class="hchips">${HOURS.map(h => `
    <div class="chip ${S.selHour===h?'selected':''}" onclick="S.selHour=${h};render()">${fmtHour(h)}</div>
  `).join('')}</div>`;

  const durChips = `<div class="chips">${DURATIONS.map(d => `
    <div class="chip ${S.selDuration===d?'selected':''}" onclick="S.selDuration=${d};render()">${fmtDur(d)}</div>
  `).join('')}</div>`;

  const rows = S.courts.length ? S.courts.map(c => {
    const free = freeHoursFor(c.id, S.date, S.selDuration);
    const available = free.length > 0;
    const price = c.pricePerHour * S.selDuration;
    return `
      <div class="ct-row ${available?'':'unavailable'}">
        <div>
          <h3>${c.name}</h3>
          <span class="tag ${c.isIndoor?'tag-indoor':'tag-outdoor'}">${c.isIndoor?'Indoor':'Outdoor'}</span>
        </div>
        <div class="meta mono" style="font-size:11px;color:var(--text-dim);text-transform:uppercase;">${c.isIndoor?'Interior · Climatizado':'Exterior · Descoberto'}</div>
        <div class="ct-slots">
          ${free.length ? free.map(h => `<div class="ct-slot-chip ${S.selHour===h?'selected':''}" onclick="S.selHour=${h};render()">${fmtHour(h)}</div>`).join('')
            : `<span class="mono" style="font-size:10.5px;color:var(--clay);text-transform:uppercase;">Indisponível</span>`}
        </div>
        <div class="ct-price-row">
          <div class="price-tag">${fmtMoney(price)}<small>${fmtDur(S.selDuration)} · ${fmtMoney(c.pricePerHour)}/h</small></div>
          <button class="btn btn-primary btn-sm" ${available?'':'disabled'} onclick="openCourt('${c.id}')">${available?'Reservar':'Indisponível'}</button>
        </div>
      </div>`;
  }).join('') : emptyState('calendar', 'Nenhum campo registado ainda.');

  const diagrams = S.courts.length ? `<div class="diagrams-row">${S.courts.map(c => {
    const available = freeHoursFor(c.id, S.date, S.selDuration).length > 0;
    return `<div class="diagram-card ${available?'avail':''}" style="width:110px;">
      <div class="box">${courtDiagram()}</div>
      <div class="lbl">${c.name}</div>
    </div>`;
  }).join('')}</div>` : '';

  return `
    <div class="header-row" style="margin-bottom:26px;">
      ${logoBlock()}
      <button class="avatar" style="border:none;cursor:pointer;" onclick="navTo('profile')">${displayName()[0].toUpperCase()}</button>
    </div>
    <div class="eyebrow">CourtMatch · ${dateObj.toLocaleDateString('pt-PT',{day:'2-digit',month:'short',year:'numeric'})}</div>
    <h2 style="font-size:26px;margin:0 0 22px;">Pesquisar campo</h2>

    <div class="split">
      <div class="col-a">
        <div class="field-label section-label">Data</div>
        <div class="chips" style="margin-bottom:16px;">
          <div class="chip ${S.date===today?'selected':''}" onclick="S.date='${today}';render()">Hoje</div>
          <div class="chip ${S.date===tomorrow?'selected':''}" onclick="S.date='${tomorrow}';render()">Amanhã</div>
        </div>
        <div class="field"><input type="date" value="${S.date}" onchange="S.date=this.value;render()"></div>

        <div class="section-label" style="margin-top:6px;">Hora de início</div>
        ${hourChips}

        <div class="section-label" style="margin-top:16px;">Duração</div>
        ${durChips}

        <div class="section" style="margin-top:8px;">
          <div class="section-label">Seleção</div>
          <div class="price-tag" style="text-align:left;font-size:24px;">${fmtHour(S.selHour)}</div>
          <div class="price-tag" style="text-align:left;font-size:24px;">${fmtDur(S.selDuration)}</div>
          <div class="meta" style="margin-top:6px;">${dateObj.toLocaleDateString('pt-PT',{day:'2-digit',month:'short',year:'numeric'})}</div>
        </div>
      </div>

      <div class="col-b">
        <div class="ct-head"><span>Campo</span><span>Tipo</span><span>Horários livres</span><span style="text-align:right;">€ / hora</span></div>
        ${rows}
        ${diagrams}
      </div>
    </div>
  `;
}

async function openCourt(id){
  S.court = S.courts.find(c => c.id === id);
  S.equipQty = {};
  go('court');
}
