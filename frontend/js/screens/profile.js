// ---------- PROFILE ----------
async function loadProfile(){
  const [history] = await Promise.all([
    api(API.game, `/api/games/history/${S.userId}`),
    loadRanking(),
  ]);
  S.gameHistory = (history || []).slice().sort((a,b) => new Date(b.scheduledAt) - new Date(a.scheduledAt));
}

function screenProfile(){
  const completed = S.gameHistory.filter(g => g.status === 'Completed');
  const wins = completed.filter(g => {
    const me = g.participants.find(p => p.userId === S.userId);
    return me && me.teamNumber === g.winningTeam;
  }).length;
  const played = completed.length;
  const winRate = played ? Math.round(wins/played*100) : 0;
  const rankPos = S.ranking.findIndex(r => r.userId === S.userId);

  const activity = S.gameHistory.length ? S.gameHistory.slice(0,8).map(g => {
    const court = S.courts.find(c => c.id === g.facilityId);
    const me = g.participants.find(p => p.userId === S.userId);
    const isCompleted = g.status === 'Completed';
    const won = isCompleted && me && me.teamNumber === g.winningTeam;
    const [sw, sl] = me ? setBalance(g, me.teamNumber) : [0,0];
    return `
      <div class="row-item" style="cursor:default;" onclick="event.stopPropagation()">
        <div>
          <h3>${g.name || court?.name || 'Jogo'}</h3>
          <div class="meta">${new Date(g.scheduledAt).toLocaleDateString('pt-PT',{day:'2-digit',month:'short'})}</div>
        </div>
        <div style="text-align:right;">
          ${isCompleted ? `<div class="price-tag" style="color:${won?'var(--gold)':'var(--clay)'}">${sw} – ${sl}</div>` : ''}
          <span class="status-pill status-${g.status}">${isCompleted ? (won?'Vitória':'Derrota') : g.status}</span>
        </div>
      </div>`;
  }).join('') : emptyState('trophy', 'Nenhum jogo disputado ainda.');

  return `
    <div class="eyebrow">Membro · Nº ${S.userId ? S.userId.slice(0,4).toUpperCase() : '----'}</div>
    <h2 style="font-size:24px;margin:0 0 22px;">Perfil</h2>

    <div class="split">
      <div class="col-a">
        <div class="section" style="border-top:2px solid var(--gold);padding-top:18px;">
          <div style="display:flex;align-items:center;gap:14px;margin-bottom:18px;">
            <div class="avatar" style="width:52px;height:52px;font-size:20px;">${displayName()[0].toUpperCase()}</div>
            <div style="min-width:0;">
              <h3 style="font-family:'Anton',sans-serif;text-transform:uppercase;font-size:16px;margin:0 0 3px;white-space:nowrap;overflow:hidden;text-overflow:ellipsis;">${displayName()}</h3>
              <div class="meta" style="margin-bottom:4px;">${S.email||''}</div>
              <div class="meta">${rankPos >= 0 ? `#${rankPos+1} no ranking` : 'Sem posição no ranking'}</div>
            </div>
          </div>
          <div style="display:flex;gap:24px;">
            <div><div class="price-tag" style="text-align:left;">${played}</div><div class="meta">Partidas</div></div>
            <div><div class="price-tag" style="text-align:left;">${wins}</div><div class="meta">Vitórias</div></div>
            <div><div class="price-tag" style="text-align:left;">${winRate}%</div><div class="meta">Taxa de vitória</div></div>
          </div>
        </div>

        <div class="section">
          <div class="section-label">Atividade recente</div>
          ${activity}
        </div>
      </div>

      <div class="col-b">
        <div class="section-label">Conta</div>
        <button class="settings-row" onclick="notReady()">
          <div class="icon-wrap">${icon('edit')}</div>
          <div class="info"><div class="n">Editar Perfil</div><div class="t">Nome, fotografia, contacto</div></div>
          <div class="chev">${icon('chevron')}</div>
        </button>
        <button class="settings-row" onclick="notReady()">
          <div class="icon-wrap">${icon('shield')}</div>
          <div class="info"><div class="n">Privacidade</div><div class="t">Visibilidade e dados</div></div>
          <div class="chev">${icon('chevron')}</div>
        </button>
        <button class="settings-row" onclick="notReady()">
          <div class="icon-wrap">${icon('bell')}</div>
          <div class="info"><div class="n">Notificações</div><div class="t">Reservas, jogos, convites</div></div>
          <div class="chev">${icon('chevron')}</div>
        </button>

        <div class="section-label" style="margin-top:22px;">Pagamentos</div>
        <button class="settings-row" onclick="notReady()">
          <div class="icon-wrap">${icon('card')}</div>
          <div class="info"><div class="n">Métodos de Pagamento</div><div class="t">Cartões e carteiras digitais</div></div>
          <div class="chev">${icon('chevron')}</div>
        </button>
        <button class="settings-row" onclick="notReady()">
          <div class="icon-wrap">${icon('receipt')}</div>
          <div class="info"><div class="n">Histórico de Faturas</div><div class="t">Reservas e alugueres</div></div>
          <div class="chev">${icon('chevron')}</div>
        </button>

        <div class="section-label" style="margin-top:22px;">Sessão</div>
        <button class="btn btn-danger" onclick="logout()">Terminar sessão</button>
      </div>
    </div>
  `;
}
