// ---------- AUTH ----------
function screenAuth(){
  const isLogin = S.authMode === 'login';
  return `
    <div style="display:flex;flex-direction:column;justify-content:center;min-height:calc(100vh - 44px);">
      <div style="text-align:center;margin-bottom:44px;">
        <div style="display:flex;justify-content:center;margin-bottom:14px;">${logoBlock()}</div>
        <p style="color:var(--text-dim);font-size:13px;margin:0;">Reserva campos. Monta o jogo. Sobe no ranking.</p>
      </div>

      <div class="tabs">
        <div class="tab ${isLogin?'active':''}" onclick="S.authMode='login';render()">Entrar</div>
        <div class="tab ${!isLogin?'active':''}" onclick="S.authMode='register';render()">Criar conta</div>
      </div>

      ${!isLogin ? `<div class="field"><label>Nome completo</label><input id="in-name" type="text" placeholder="Seu nome"></div>` : ''}
      <div class="field"><label>E-mail</label><input id="in-email" type="email" placeholder="voce@email.com" value=""></div>
      <div class="field"><label>Senha</label><input id="in-pass" type="password" placeholder="••••••••"></div>
      <button class="btn btn-primary" onclick="submitAuth()">${isLogin?'Entrar':'Criar conta e entrar'}</button>
      <p class="mono" style="text-align:center;font-size:10px;color:var(--text-dim);margin-top:18px;text-transform:uppercase;letter-spacing:0.05em;">
        Ambiente de teste local — dados gravados no LocalDB
      </p>
    </div>`;
}

async function submitAuth(){
  const email = document.getElementById('in-email').value.trim();
  const password = document.getElementById('in-pass').value;
  const nameEl = document.getElementById('in-name');
  const fullName = nameEl ? nameEl.value.trim() : '';
  if (!email || !password){ toast('Preencha e-mail e senha', 'error'); return; }
  if (S.authMode === 'register' && !fullName){ toast('Informe seu nome', 'error'); return; }

  try{
    if (S.authMode === 'register'){
      await api(API.identity, '/api/auth/register', { method:'POST', auth:false, body:{ fullName, email, password } });
      toast('Conta criada! Entrando...', 'ok');
    }
    const res = await api(API.identity, '/api/auth/login', { method:'POST', auth:false, body:{ email, password } });
    S.token = res.token;
    const claims = decodeJwt(res.token) || {};
    S.userId = claims.nameid || claims.sub;
    S.email = email;
    S.fullName = claims.name || '';
    S.isAdmin = hasAdminRole(claims);
    store.set('cm_token', S.token);
    store.set('cm_userId', S.userId);
    store.set('cm_email', S.email);
    store.set('cm_fullName', S.fullName);
    store.set('cm_isAdmin', S.isAdmin ? '1' : '0');
    await loadCatalog();
    go('home');
  } catch(e){ /* toast already shown */ }
}
