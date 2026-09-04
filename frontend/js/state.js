// ---------- STATE ----------
// Persistência local (sobrevive a reload) e o estado global único da app (S).
const store = {
  get(k){ try{ return localStorage.getItem(k); } catch(e){ return null; } },
  set(k,v){ try{ localStorage.setItem(k,v); } catch(e){} },
  remove(k){ try{ localStorage.removeItem(k); } catch(e){} },
};

const S = {
  token: store.get('cm_token') || null,
  userId: store.get('cm_userId') || null,
  email: store.get('cm_email') || null,
  fullName: store.get('cm_fullName') || '',
  isAdmin: store.get('cm_isAdmin') === '1',
  view: null,
  authMode: 'login',
  courts: [], equipments: [], timeslots: [], allBookings: [],
  date: '', selHour: 18, selDuration: 2,
  court: null,
  equipQty: {},
  addEquipQty: {},
  bookings: [],
  booking: null,
  bookingGames: [],
  game: null,
  ranking: [],
  gameHistory: [],
  userNames: {},
  inviteTeam: 1,
  sets: [{ setNumber: 1, teamOneGames: 0, teamTwoGames: 0 }],
  newGame: null,
  adminTab: 'courts',
  adminEdit: { type: null, id: null },
};
