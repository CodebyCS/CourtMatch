// ---------- NEW GAME ----------
function openNewGame(){
  S.newGame = {
    bookingId: S.booking.id, courtId: S.booking.courtId, scheduledAt: S.booking.startTime,
    name: 'Jogo Amigável', format: 'Doubles',
    players: [{ userId: S.userId, team: 1 }],
    searchResults: [],
  };
  go('newgame');
}

function maxPerTeam(){ return S.newGame.format === 'Singles' ? 1 : 2; }

function screenNewGame(){
  const ng = S.newGame;
  const cap = maxPerTeam();
  const court = S.courts.find(c => c.id === ng.courtId);

  const playerChips = ng.players.map(p => `
    <div class="chip chip-removable ${p.userId===S.userId?'selected':''}">
      ${p.userId===S.userId ? '⚡ Eu' : nameFor(p.userId)}
      ${p.userId!==S.userId ? `<button onclick="removeStagedPlayer('${p.userId}')">×</button>` : ''}
    </div>
  `).join('');

  const resultsHtml = (ng.searchResults||[]).length ? ng.searchResults.map(u => {
    const already = ng.players.some(p => p.userId === u.id);
    const full = ng.players.length >= cap*2;
    return `
      <div class="invite-row">
        <div class="avatar" style="width:32px;height:32px;font-size:12px;">${(u.fullName||u.email||'?')[0].toUpperCase()}</div>
        <div class="info"><div class="n">${u.fullName || u.email}</div><div class="t">Membro CourtMatch</div></div>
        <button class="btn btn-ghost btn-sm" ${already||full?'disabled':''} onclick="addStagedPlayer('${u.id}','${(u.fullName||u.email).replace(/'/g,"")}')">${already?'Adicionado':'+ Convidar'}</button>
      </div>`;
  }).join('') : `<p class="mono" style="color:var(--text-dim);font-size:11px;">Pesquisa pelo nome ou e-mail do jogador.</p>`;

  return `
    ${backLink('booking','Voltar')}
    <div class="eyebrow">${court ? court.name : ''} · ${fmtDate(ng.scheduledAt)}</div>
    <h2 style="font-size:26px;margin:0 0 22px;">Novo jogo</h2>

    <div class="split">
      <div class="col-a">
        <div class="field"><label>Nome do jogo</label><input id="ng-name" type="text" value="${ng.name}" onchange="S.newGame.name=this.value"></div>

        <div class="section-label">Formato de jogo</div>
        <div class="format-grid" style="margin-bottom:20px;">
          <div class="format-tile ${ng.format==='Doubles'?'selected':''}" onclick="setGameFormat('Doubles')">
            <div class="big">2×2</div><div class="sub">Duplas</div>
          </div>
          <div class="format-tile ${ng.format==='Singles'?'selected':''}" onclick="setGameFormat('Singles')">
            <div class="big">1×1</div><div class="sub">Individual</div>
          </div>
        </div>

        <div class="section-label">Jogadores adicionados (${ng.players.length})</div>
        <div class="chips" style="margin-bottom:20px;">${playerChips}</div>

        <button class="btn btn-primary" onclick="saveNewGame()">Guardar jogo</button>
      </div>

      <div class="col-b">
        <div class="section-label">Convidar jogadores</div>
        <div class="field"><input type="text" placeholder="Pesquisar por nome ou e-mail..." oninput="onNewGameSearch(this.value)"></div>
        <div id="ng-results">${resultsHtml}</div>
      </div>
    </div>
  `;
}

function setGameFormat(fmt){
  S.newGame.format = fmt;
  const cap = maxPerTeam();
  // trim players beyond the new cap per team, keeping self
  const byTeam = { 1: [], 2: [] };
  S.newGame.players.forEach(p => byTeam[p.team].push(p));
  S.newGame.players = [...byTeam[1].slice(0,cap), ...byTeam[2].slice(0,cap)];
  render();
}

let ngSearchTimer = null;
function onNewGameSearch(term){
  clearTimeout(ngSearchTimer);
  ngSearchTimer = setTimeout(() => doNewGameSearch(term), 300);
}
async function doNewGameSearch(term){
  term = (term||'').trim();
  if (!term){ S.newGame.searchResults = []; document.getElementById('ng-results').innerHTML = '<p class="mono" style="color:var(--text-dim);font-size:11px;">Pesquisa pelo nome ou e-mail do jogador.</p>'; return; }
  try{
    const res = await fetch(API.identity + '/api/auth/users?search=' + encodeURIComponent(term) + '&limit=15');
    const users = res.ok ? await res.json() : [];
    S.newGame.searchResults = users.filter(u => u.id !== S.userId);
    render();
  } catch(e){}
}

function addStagedPlayer(id, name){
  const cap = maxPerTeam();
  const byTeam = { 1: 0, 2: 0 };
  S.newGame.players.forEach(p => byTeam[p.team]++);
  let team = byTeam[2] < cap ? 2 : (byTeam[1] < cap ? 1 : null);
  if (!team){ toast('As equipas já estão completas para este formato', 'error'); return; }
  S.newGame.players.push({ userId: id, team });
  S.userNames[id] = name;
  render();
}

function removeStagedPlayer(id){
  S.newGame.players = S.newGame.players.filter(p => p.userId !== id);
  render();
}

async function saveNewGame(){
  const ng = S.newGame;
  try{
    S.game = await api(API.game, '/api/games', { method:'POST', body:{
      bookingId: ng.bookingId, facilityId: ng.courtId, scheduledAt: ng.scheduledAt,
      name: ng.name, format: ng.format,
      participants: ng.players.map(p => ({ userId: p.userId, teamNumber: p.team }))
    }});
    S.bookingGames.push(S.game);
    S.sets = [{ setNumber: 1, teamOneGames: 0, teamTwoGames: 0 }];
    toast('Jogo criado!', 'ok');
    go('game');
  } catch(e){}
}
