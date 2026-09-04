// ---------- ROUTER (navegação e shell da app) ----------
function go(view){ S.view = view; render(); }

function logout(){
  S.token = null; S.userId = null; S.email = null; S.fullName = ''; S.isAdmin = false;
  store.remove('cm_token'); store.remove('cm_userId'); store.remove('cm_email'); store.remove('cm_fullName'); store.remove('cm_isAdmin');
  go('auth');
}

function visibleNavItems(){
  const items = [
    { v:'home', i:'home', l:'Início' },
    { v:'mybookings', i:'calendar', l:'Reservas' },
    { v:'ranking', i:'trophy', l:'Ranking' },
    { v:'profile', i:'user', l:'Perfil' },
  ];
  if (S.isAdmin) items.push({ v:'admin', i:'settings', l:'Gestão' });
  return items;
}

function bottomNav(){
  return `<div class="bottom-nav">
    ${visibleNavItems().map(it => `
      <button class="nav-item ${S.view===it.v?'active':''}" onclick="navTo('${it.v}')">${icon(it.i)}<span>${it.l}</span></button>
    `).join('')}
  </div>`;
}

function sidebar(){
  return `
    ${logoBlock()}
    <div class="sb-section-label">Menu principal</div>
    <nav class="sb-nav">
      ${visibleNavItems().map(it => `
        <button class="sb-nav-item ${S.view===it.v?'active':''}" onclick="navTo('${it.v}')">${icon(it.i)}${it.l}</button>
      `).join('')}
    </nav>
    <button class="sb-user" onclick="navTo('profile')">
      <div class="avatar">${displayName()[0].toUpperCase()}</div>
      <div class="info"><div class="n">${displayName()}</div><div class="t">${S.isAdmin?'Admin':'Membro'}</div></div>
    </button>
  `;
}

async function navTo(view){
  if (view === 'mybookings') await loadMyBookings();
  if (view === 'ranking') await loadRanking();
  if (view === 'profile') await loadProfile();
  if (view === 'admin') await loadAdmin();
  go(view);
}

const authedViews = ['home','court','payment','mybookings','booking','addequip','newgame','game','ranking','profile','admin'];

async function render(){
  const screenEl = document.getElementById('screen');
  const navSlot = document.getElementById('nav-slot');
  const sidebarEl = document.getElementById('sidebar');

  const renderers = {
    auth: screenAuth,
    home: screenHome,
    court: screenCourt,
    payment: screenPayment,
    mybookings: screenMyBookings,
    booking: screenBookingDetail,
    addequip: screenAddEquipment,
    newgame: screenNewGame,
    game: screenGame,
    ranking: screenRanking,
    profile: screenProfile,
    admin: screenAdmin,
  };

  screenEl.innerHTML = renderers[S.view] ? renderers[S.view]() : '<div class="loader"><span class="dot"></span><span class="dot"></span><span class="dot"></span></div>';
  navSlot.innerHTML = authedViews.includes(S.view) ? bottomNav() : '';
  sidebarEl.innerHTML = authedViews.includes(S.view) ? sidebar() : '';
}
