// ---------- GAME ----------
function screenGame(){
  const g = S.game;
  const teams = [1,2].map(team => {
    const players = g.participants.filter(p => p.teamNumber === team);
    return `
      <div class="section">
        <div class="section-label">Equipa ${team === 1 ? 'A' : 'B'}</div>
        ${players.length ? players.map(p => `
          <div class="equip-row">
            <div class="name" style="font-size:12.5px;">${nameFor(p.userId)}</div>
            <div style="display:flex;align-items:center;gap:8px;">
              <span class="status-pill status-${p.status}">${p.status}</span>
              ${p.userId === S.userId && p.status === 'Invited' ? `
                <button class="btn btn-sm btn-primary" onclick="respondInvite(true)">Aceitar</button>
              ` : ''}
            </div>
          </div>`).join('') : `<div class="mono" style="color:var(--text-dim);font-size:11.5px;">Ninguém ainda</div>`}
      </div>`;
  }).join('');

  return `
    ${backLink('booking','Voltar')}
    <div class="header-row">
      <div class="eyebrow" style="margin:0;flex:1;">${g.format === 'Singles' ? '1×1 Individual' : '2×2 Duplas'}</div>
      <span class="status-pill status-${g.status}">${g.status}</span>
    </div>
    <h2 style="font-size:26px;margin:0 0 22px;">${g.name || 'Jogo'}</h2>

    <div class="split">
      <div class="col-a">
        ${teams}
      </div>
      <div class="col-b">
        ${g.status !== 'Completed' && g.status !== 'Cancelled' ? `
          <div class="section" style="border-top:none;padding-top:0;">
            <div class="section-label">Convidar jogador</div>
            <div class="field" style="margin-bottom:10px;">
              <label>Pesquisar por nome ou e-mail</label>
              <input type="text" placeholder="Escreve para pesquisar..." oninput="onInviteSearchInput(this.value)">
            </div>
            <div id="invite-results"><p class="mono" style="color:var(--text-dim);font-size:11px;">Pesquisa pelo nome ou e-mail do jogador.</p></div>
            <div class="chips" style="margin:12px 0 14px;">
              <div class="chip ${S.inviteTeam===1?'selected':''}" onclick="S.inviteTeam=1;render()">Equipa A</div>
              <div class="chip ${S.inviteTeam===2?'selected':''}" onclick="S.inviteTeam=2;render()">Equipa B</div>
            </div>
          </div>
        ` : ''}

        ${g.status !== 'Cancelled' ? `
          <div class="section">
            <div class="section-label">${g.sets && g.sets.length ? 'Editar resultado' : 'Registar resultado'}</div>
            ${S.sets.map((s,i) => `
              <div class="set-row">
                <span style="width:40px;">Set ${s.setNumber}</span>
                <input type="number" min="0" value="${s.teamOneGames}" onchange="S.sets[${i}].teamOneGames=+this.value">
                <span>×</span>
                <input type="number" min="0" value="${s.teamTwoGames}" onchange="S.sets[${i}].teamTwoGames=+this.value">
              </div>
            `).join('')}
            <div style="display:flex;gap:8px;margin-top:8px;">
              <button class="btn btn-ghost btn-sm" onclick="addSet()">+ Set</button>
              <button class="btn btn-primary btn-sm" style="flex:1;" onclick="registerResult()">Guardar resultado</button>
            </div>
          </div>
        ` : ''}
      </div>
    </div>
  `;
}

let inviteSearchTimer = null;
function onInviteSearchInput(term){
  clearTimeout(inviteSearchTimer);
  inviteSearchTimer = setTimeout(() => doInviteSearch(term), 300);
}

async function doInviteSearch(term){
  const el = document.getElementById('invite-results');
  if (!el) return;
  term = (term || '').trim();
  if (!term){ el.innerHTML = '<p class="mono" style="color:var(--text-dim);font-size:11px;">Pesquisa pelo nome ou e-mail do jogador.</p>'; return; }
  el.innerHTML = '<p class="mono" style="color:var(--text-dim);font-size:11px;">A pesquisar...</p>';
  try{
    const res = await fetch(API.identity + '/api/auth/users?search=' + encodeURIComponent(term) + '&limit=15');
    const users = res.ok ? await res.json() : [];
    const filtered = users.filter(u => u.id !== S.userId && !S.game.participants.some(p => p.userId === u.id));
    el.innerHTML = filtered.length
      ? filtered.map(u => `
          <div class="invite-row">
            <div class="avatar" style="width:32px;height:32px;font-size:12px;">${(u.fullName||u.email||'?')[0].toUpperCase()}</div>
            <div class="info"><div class="n">${u.fullName || u.email}</div><div class="t">Membro CourtMatch</div></div>
            <button class="btn btn-ghost btn-sm" onclick="invitePlayer('${u.id}')">+ Convidar</button>
          </div>`).join('')
      : '<p class="mono" style="color:var(--text-dim);font-size:11px;">Nenhum jogador encontrado.</p>';
  } catch(e){
    el.innerHTML = '<p class="mono" style="color:var(--clay);font-size:11px;">Erro ao pesquisar.</p>';
  }
}

function syncBookingGame(){
  const idx = S.bookingGames.findIndex(g => g.id === S.game.id);
  if (idx >= 0) S.bookingGames[idx] = S.game;
}

async function invitePlayer(userId){
  try{
    S.game = await api(API.game, `/api/games/${S.game.id}/invite`, { method:'POST', body:{ userId, teamNumber: S.inviteTeam } });
    syncBookingGame();
    await resolveUserNames([userId]);
    toast('Convite enviado!', 'ok');
    render();
  } catch(e){}
}

async function respondInvite(confirm){
  try{
    const action = confirm ? 'confirm' : 'decline';
    S.game = await api(API.game, `/api/games/${S.game.id}/players/${S.userId}/${action}`, { method:'POST' });
    syncBookingGame();
    toast(confirm ? 'Participação confirmada!' : 'Convite recusado.', 'ok');
    render();
  } catch(e){}
}

function addSet(){
  S.sets.push({ setNumber: S.sets.length + 1, teamOneGames: 0, teamTwoGames: 0 });
  render();
}

async function registerResult(){
  try{
    S.game = await api(API.game, `/api/games/${S.game.id}/result`, { method:'POST', body:{ sets: S.sets } });
    syncBookingGame();
    S.sets = S.game.sets.map(s => ({ setNumber:s.setNumber, teamOneGames:s.teamOneGames, teamTwoGames:s.teamTwoGames }));
    toast('Resultado guardado!', 'ok');
    render();
  } catch(e){}
}
