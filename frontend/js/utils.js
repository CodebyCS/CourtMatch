// ---------- UTILS (helpers puros e de apresentação) ----------

function icon(name){
  const paths = {
    back: '<path d="M15 18l-6-6 6-6"/>',
    logout: '<path d="M9 21H5a2 2 0 01-2-2V5a2 2 0 012-2h4"/><path d="M16 17l5-5-5-5"/><path d="M21 12H9"/>',
    home: '<path d="M3 9l9-7 9 7"/><path d="M9 22V12h6v10"/><path d="M4 22h16"/>',
    calendar: '<rect x="3" y="4" width="18" height="18" rx="1"/><path d="M16 2v4M8 2v4M3 10h18"/>',
    trophy: '<path d="M8 21h8M12 17v4M7 4h10v5a5 5 0 01-10 0V4z"/><path d="M7 5H4a1 1 0 00-1 1v1a4 4 0 004 4M17 5h3a1 1 0 011 1v1a4 4 0 01-4 4"/>',
    racket: '<circle cx="12" cy="8" r="5"/><path d="M12 13v8M9 21h6"/>',
    plus: '<path d="M12 5v14M5 12h14"/>',
    user: '<circle cx="12" cy="8" r="4"/><path d="M4 21c0-4 4-6 8-6s8 2 8 6"/>',
    check: '<path d="M20 6L9 17l-5-5"/>',
    phone: '<rect x="6" y="2" width="12" height="20" rx="2"/><path d="M10 18h4"/>',
    card: '<rect x="2" y="5" width="20" height="14" rx="2"/><path d="M2 10h20"/>',
    apple: '<path d="M16 3c-1 1-2.5 1.7-4 1.5C11.6 3 12.8 1 14.5 1c.3 1-.2 2-1.5 2M6 8c-2.5 1.5-3 5-1 8.5 1.5 2.5 3 4.5 5 4.5 1.3 0 1.8-.8 3.5-.8s2.1.8 3.5.8c1.7 0 3.2-2 4.5-4.5-3-1.5-3.5-6-.5-7.5-1.5-2-4-2.5-5-2.5-1.5 0-2.8.9-4 .9S7.5 6.5 6 8z"/>',
    edit: '<path d="M12 20h9"/><path d="M16.5 3.5a2.1 2.1 0 013 3L7 19l-4 1 1-4z"/>',
    shield: '<path d="M12 2l8 4v6c0 5-3.4 8.4-8 10-4.6-1.6-8-5-8-10V6z"/>',
    bell: '<path d="M6 8a6 6 0 0112 0c0 5 2 6 2 6H4s2-1 2-6"/><path d="M10 21a2 2 0 004 0"/>',
    receipt: '<path d="M4 2h16v20l-3-2-3 2-3-2-3 2-3-2-1 2z"/><path d="M8 7h8M8 11h8M8 15h5"/>',
    chevron: '<path d="M9 18l6-6-6-6"/>',
    settings: '<circle cx="12" cy="12" r="3"/><path d="M19.4 15a1.65 1.65 0 00.33 1.82l.06.06a2 2 0 11-2.83 2.83l-.06-.06a1.65 1.65 0 00-1.82-.33 1.65 1.65 0 00-1 1.51V21a2 2 0 01-4 0v-.09a1.65 1.65 0 00-1-1.51 1.65 1.65 0 00-1.82.33l-.06.06a2 2 0 11-2.83-2.83l.06-.06a1.65 1.65 0 00.33-1.82 1.65 1.65 0 00-1.51-1H3a2 2 0 010-4h.09a1.65 1.65 0 001.51-1 1.65 1.65 0 00-.33-1.82l-.06-.06a2 2 0 112.83-2.83l.06.06a1.65 1.65 0 001.82.33h0a1.65 1.65 0 001-1.51V3a2 2 0 014 0v.09a1.65 1.65 0 001 1.51h0a1.65 1.65 0 001.82-.33l.06-.06a2 2 0 112.83 2.83l-.06.06a1.65 1.65 0 00-.33 1.82v0a1.65 1.65 0 001.51 1H21a2 2 0 010 4h-.09a1.65 1.65 0 00-1.51 1z"/>',
  };
  return `<svg viewBox="0 0 24 24" fill="none" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">${paths[name]||''}</svg>`;
}

function courtDiagram(){
  return `<svg class="court-svg" viewBox="0 0 100 68" width="100%" height="auto" fill="none" stroke="currentColor" stroke-width="1.6">
    <rect x="2" y="2" width="96" height="64" rx="2"/>
    <line x1="2" y1="34" x2="98" y2="34"/>
    <rect x="2" y="17" width="27" height="34"/>
    <rect x="71" y="17" width="27" height="34"/>
    <line x1="50" y1="2" x2="50" y2="66" stroke-dasharray="2 3" opacity="0.5"/>
  </svg>`;
}

function toast(msg, type='ok'){
  const wrap = document.getElementById('toast-wrap');
  const el = document.createElement('div');
  el.className = 'toast ' + type;
  el.textContent = msg;
  wrap.appendChild(el);
  setTimeout(()=>{ el.remove(); }, 3200);
}

function decodeJwt(token){
  try{
    const payload = token.split('.')[1];
    const json = decodeURIComponent(atob(payload.replace(/-/g,'+').replace(/_/g,'/')).split('').map(c =>
      '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2)).join(''));
    return JSON.parse(json);
  } catch(e){ return null; }
}

function hasAdminRole(claims){
  const role = claims && claims.role;
  if (!role) return false;
  return Array.isArray(role) ? role.includes('Admin') : role === 'Admin';
}

function fmtMoney(v){ return Number(v).toFixed(2).replace('.', ',') + ' €'; }
function fmtHour(h){ const hh=Math.floor(h), mm=Math.round((h-hh)*60); return String(hh).padStart(2,'0')+':'+String(mm).padStart(2,'0'); }
function fmtDur(h){ return h===Math.floor(h) ? h+'h' : (h-0.5)+'h30'; }
function fmtDate(iso){
  const d = new Date(iso);
  return d.toLocaleDateString('pt-PT', { day:'2-digit', month:'short' }) + ' · ' +
    d.toLocaleTimeString('pt-PT', { hour:'2-digit', minute:'2-digit' });
}

function displayName(){ return S.fullName || S.email || '?'; }

function nameFor(userId){
  if (userId === S.userId) return 'Eu';
  return S.userNames[userId] || userId;
}

function logoBlock(){
  return `<div class="logo"><div class="bar"></div><div class="logo-text"><span class="name">CourtMatch</span><span class="tagline">Clube de Padel</span></div></div>`;
}

function backLink(view, label){
  return `<button class="back-link" onclick="go('${view}')">${icon('back')}${label}</button>`;
}

function emptyState(iconName, text){
  return `<div class="empty">${icon(iconName)}<p>${text}</p></div>`;
}

function scoreLine(g){
  const t1 = g.sets.filter(s => s.teamOneGames > s.teamTwoGames).length;
  const t2 = g.sets.length - t1;
  return `${t1} – ${t2}`;
}

function setBalance(g, myTeam){
  let won=0, lost=0;
  (g.sets||[]).forEach(s => {
    const mine = myTeam===1 ? s.teamOneGames : s.teamTwoGames;
    const theirs = myTeam===1 ? s.teamTwoGames : s.teamOneGames;
    if (mine>theirs) won++; else if (theirs>mine) lost++;
  });
  return [won,lost];
}

function notReady(){ toast('Em breve — ainda não existe na API', 'ok'); }
