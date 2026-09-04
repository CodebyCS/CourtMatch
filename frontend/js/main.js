// ---------- MAIN (arranque da app) ----------
async function boot(){
  if (S.token){
    try{
      await loadCatalog();
      go('home');
      return;
    } catch(e){ logout(); return; }
  }
  go('auth');
}

document.addEventListener('change', (ev) => {
  if (ev.target && ev.target.id === 'af-eq-cat'){
    const wrap = document.getElementById('af-eq-cat-custom-wrap');
    if (wrap) wrap.style.display = ev.target.value === '__custom__' ? 'block' : 'none';
  }
});

boot();
