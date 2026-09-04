// ---------- API (camada de acesso a dados) ----------
// Único ponto de contacto com as 4 APIs. Nenhuma tela faz fetch() diretamente
// fora daqui (as duas exceções — getGamesForBooking e resolveUserNames — usam
// fetch cru porque toleram falha silenciosa, sem toast de erro).

async function api(base, path, { method='GET', body, auth=true } = {}){
  const headers = { 'Content-Type': 'application/json' };
  if (auth && S.token) headers['Authorization'] = 'Bearer ' + S.token;
  let res;
  try{
    res = await fetch(base + path, { method, headers, body: body ? JSON.stringify(body) : undefined, mode: 'cors' });
  } catch(e){
    toast('Não foi possível conectar em ' + base + ' — a API está rodando?', 'error');
    throw e;
  }
  const text = await res.text();
  let data = null;
  try{ data = text ? JSON.parse(text) : null; } catch(e){ data = text; }
  if (!res.ok){
    const msg = (data && (data.message || (Array.isArray(data) ? data.map(d=>d.description).join(', ') : JSON.stringify(data)))) || res.statusText;
    toast(msg, 'error');
    throw new Error(msg);
  }
  return data;
}

async function resolveUserNames(ids){
  const missing = [...new Set(ids)].filter(id => id && id !== S.userId && !S.userNames[id]);
  if (!missing.length) return;
  try{
    const res = await fetch(API.identity + '/api/auth/users/by-ids?ids=' + encodeURIComponent(missing.join(',')));
    if (!res.ok) return;
    const users = await res.json();
    users.forEach(u => { S.userNames[u.id] = u.fullName || u.email; });
  } catch(e){}
}

async function getGamesForBooking(bookingId){
  try{
    const res = await fetch(API.game + '/api/games/by-booking/' + bookingId, {
      headers: S.token ? { 'Authorization': 'Bearer ' + S.token } : {}
    });
    if (!res.ok) return [];
    return await res.json();
  } catch(e){ return []; }
}
