// ---------- RANKING ----------
async function loadRanking(){
  S.ranking = await api(API.game, '/api/ranking?top=20');
  await resolveUserNames(S.ranking.map(r => r.userId));
}

function screenRanking(){
  const rows = S.ranking.length ? S.ranking.map((r,i) => `
    <div class="rank-row">
      <div class="rank-num ${i===0?'top1':''}">${i+1}</div>
      <div class="rank-info">
        <div class="rank-uid">${nameFor(r.userId)}</div>
        <div class="rank-sub">${r.gamesWon}V ${r.gamesPlayed - r.gamesWon}D · ${Math.round(r.winRate)}% vit.</div>
      </div>
      <div class="rank-pts">${r.rankingPoints}</div>
    </div>
  `).join('') : emptyState('trophy', 'Ninguém no ranking ainda — complete um jogo para aparecer aqui.');

  return `
    <div class="eyebrow">Clube</div>
    <h2 style="font-size:24px;margin:0 0 22px;">Ranking</h2>
    ${rows}
  `;
}
