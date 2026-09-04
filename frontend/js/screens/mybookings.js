// ---------- MY BOOKINGS ----------
async function loadMyBookings(){
  const all = await api(API.booking, '/api/bookings?startDate=2020-01-01T00:00:00&endDate=2035-01-01T00:00:00');
  S.bookings = (all||[]).filter(b => b.hostPlayerId?.toLowerCase() === S.userId?.toLowerCase())
    .sort((a,b) => new Date(b.startTime) - new Date(a.startTime));
}

function screenMyBookings(){
  const list = S.bookings.length ? S.bookings.map(b => {
    const court = S.courts.find(c => c.id === b.courtId);
    return `
      <div class="row-item" onclick="openBookingDetail('${b.id}')">
        <div>
          <h3>${court ? court.name : 'Campo'}</h3>
          <div class="meta">${fmtDate(b.startTime)}</div>
        </div>
        <span class="status-pill status-${b.status}">${b.status}</span>
      </div>`;
  }).join('') : emptyState('calendar', 'Ainda não tens reservas.');

  return `
    <div class="eyebrow">Programação</div>
    <h2 style="font-size:24px;margin:0 0 22px;">Minhas reservas</h2>
    ${list}
  `;
}
