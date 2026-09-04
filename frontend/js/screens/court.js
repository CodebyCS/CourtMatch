// ---------- COURT (equipment step) + PAYMENT ----------
function equipTotal(){
  return Object.entries(S.equipQty).reduce((sum,[id,qty]) => {
    const e = S.equipments.find(x => x.id === id);
    return sum + (e ? e.rentalPrice * qty : 0);
  }, 0);
}

function bookingTimes(){
  const startTime = `${S.date}T${fmtHour(S.selHour)}:00`;
  const end = S.selHour + S.selDuration;
  const endTime = `${S.date}T${fmtHour(end)}:00`;
  return { startTime, endTime };
}

function screenCourt(){
  const c = S.court;
  const courtPrice = c.pricePerHour * S.selDuration;
  const total = courtPrice + equipTotal();

  const equipRows = S.equipments.length ? S.equipments.map(e => `
      <div class="equip-row">
        <div>
          <div class="name">${e.name}</div>
          <div class="sub">${fmtMoney(e.rentalPrice)} · stock ${e.stock}</div>
        </div>
        <div class="stepper">
          <button onclick="stepEquip('${e.id}',-1)">−</button>
          <span class="qty">${S.equipQty[e.id]||0}</span>
          <button onclick="stepEquip('${e.id}',1)">+</button>
        </div>
      </div>`).join('') : `<p class="mono" style="color:var(--text-dim);font-size:11.5px;">Nenhum equipamento registado.</p>`;

  return `
    ${backLink('home','Voltar')}
    <div class="eyebrow">Campo ${c.name} · ${fmtHour(S.selHour)}</div>
    <h2 style="font-size:26px;margin:0 0 8px;">Equipamento</h2>
    <span class="tag ${c.isIndoor?'tag-indoor':'tag-outdoor'}">${c.isIndoor?'Indoor':'Outdoor'}</span>

    <div class="section" style="margin-top:18px;">
      <div class="section-label">Adicionar equipamento (opcional)</div>
      ${equipRows}
    </div>

    <div class="section">
      <div class="summary-row"><span>Campo (${fmtDur(S.selDuration)} × ${fmtMoney(c.pricePerHour)})</span><span>${fmtMoney(courtPrice)}</span></div>
      <div class="summary-row"><span>Equipamentos</span><span>${fmtMoney(equipTotal())}</span></div>
      <div class="summary-row total"><span>Total</span><span>${fmtMoney(total)}</span></div>
    </div>

    <button class="btn btn-primary" style="margin-top:8px;" onclick="go('payment')">Ir para pagamento</button>
  `;
}

function stepEquip(id, delta){
  const cur = S.equipQty[id] || 0;
  const next = Math.max(0, cur + delta);
  if (next === 0) delete S.equipQty[id]; else S.equipQty[id] = next;
  render();
}

function screenPayment(){
  const c = S.court;
  const courtPrice = c.pricePerHour * S.selDuration;
  const total = courtPrice + equipTotal();
  const dateObj = new Date(S.date + 'T00:00:00');

  return `
    ${backLink('court','Cancelar')}
    <div class="eyebrow">Reserva · ${c.name} — ${c.isIndoor?'Indoor':'Outdoor'}</div>
    <h2 style="font-size:26px;margin:0 0 22px;">Confirmar reserva</h2>

    <div class="split">
      <div class="col-a">
        <div class="section" style="border-top:none;padding-top:0;">
          <div class="summary-row"><span>Campo</span><span>${c.name}</span></div>
          <div class="summary-row"><span>Tipo</span><span>${c.isIndoor?'Interior · Climatizado':'Exterior · Descoberto'}</span></div>
          <div class="summary-row"><span>Data</span><span>${dateObj.toLocaleDateString('pt-PT',{day:'2-digit',month:'short',year:'numeric'})}</span></div>
          <div class="summary-row"><span>Horário</span><span>${fmtHour(S.selHour)} – ${fmtDur(S.selDuration)}</span></div>
          <div class="summary-row total"><span>Total</span><span>${fmtMoney(total)}</span></div>
        </div>
        <div style="width:140px;color:var(--text-dim);margin-top:10px;">${courtDiagram()}</div>
      </div>
      <div class="col-b">
        <button class="btn btn-primary" onclick="submitBooking()">Confirmar reserva · ${fmtMoney(total)}</button>
      </div>
    </div>
  `;
}

async function submitBooking(){
  const c = S.court;
  const { startTime, endTime } = bookingTimes();
  const courtPrice = c.pricePerHour * S.selDuration;
  const total = courtPrice + equipTotal();

  try{
    const created = await api(API.booking, '/api/bookings', { method:'POST', body:{
      courtId: c.id, hostPlayerId: S.userId, startTime, endTime, courtPrice, totalPrice: total
    }});

    for (const [equipId, qty] of Object.entries(S.equipQty)){
      const e = S.equipments.find(x => x.id === equipId);
      await api(API.booking, `/api/bookings/${created.id}/equipments`, { method:'POST', body:{
        equipmentId: equipId, quantity: qty, unitPrice: e.rentalPrice
      }});
    }
    S.allBookings.push(created);

    toast('Reserva criada!', 'ok');
    await openBookingDetail(created.id);
  } catch(e){ /* toast already shown */ }
}
