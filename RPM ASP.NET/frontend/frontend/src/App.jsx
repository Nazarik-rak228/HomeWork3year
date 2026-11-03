import { useState } from 'react';

// === ЗАМЕНИ на свой адрес API ===
const API_URL = 'https://localhost:7070';   // ← сюда свой порт

export default function App() {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [token, setToken] = useState(localStorage.getItem('token') || '');
  const [message, setMessage] = useState('');
  const [error, setError] = useState('');

  // Вход
  const handleLogin = async () => {
    setError('');
    setMessage('');

    try {
      const res = await fetch(`${API_URL}/api/auth/login`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ username, password })
      });

      if (res.status === 429) {
        setError('Слишком много попыток (429). Подожди минуту.');
        return;
      }

      if (!res.ok) {
        const data = await res.json().catch(() => ({}));
        setError(data.error || `Ошибка входа: ${res.status}`);
        return;
      }

      const data = await res.json();
      setToken(data.accessToken);
      localStorage.setItem('token', data.accessToken);
      setMessage('Успешный вход! Токен сохранён.');
    } catch (err) {
      setError('Ошибка CORS или сети: ' + err.message);
    }
  };

  // Запрос к защищённому эндпоинту
  const handleProtectedRequest = async () => {
    setError('');
    setMessage('');

    if (!token) {
      setError('Сначала войди');
      return;
    }

    try {
      const res = await fetch(`${API_URL}/api/order`, {
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });

      if (res.status === 401) {
        setError('401 Unauthorized — токен невалидный');
        return;
 dual;
      }
      if (res.status === 403) {
        setError('403 Forbidden — нет прав');
        return;
      }
      if (!res.ok) {
        setError(`Ошибка ${res.status}`);
        return;
      }

      const data = await res.json();
      setMessage('Защищённый запрос успешен!\n\n' + JSON.stringify(data, null, 2));
    } catch (err) {
      setError('Ошибка CORS или сети: ' + err.message);
    }
  };

  const handleLogout = () => {
    setToken('');
    localStorage.removeItem('token');
    setMessage('Вышел');
  };

  return (
    <div style={{ padding: 40, fontFamily: 'Arial', maxWidth: 500 }}>
      <h1>JWT + CORS Demo</h1>

      {!token ? (
        <div>
          <input
            placeholder="Логин"
            value={username}
            onChange={e => setUsername(e.target.value)}
            style={{ display: 'block', width: '100%', padding: 8, marginBottom: 10 }}
          />
          <input
            type="password"
            placeholder="Пароль"
            value={password}
            onChange={e => setPassword(e.target.value)}
            style={{ display: 'block', width: '100%', padding: 8, marginBottom: 10 }}
          />
          <button onClick={handleLogin}>Войти</button>
        </div>
      ) : (
        <div>
          <p>Токен сохранён</p>
          <button onClick={handleProtectedRequest} style={{ marginRight: 10 }}>
            Запросить /api/order
          </button>
          <button onClick={handleLogout}>Выйти</button>
        </div>
      )}

      {error && (
        <pre style={{ color: 'red', background: '#ffeaea', padding: 12, marginTop: 20 }}>
          {error}
        </pre>
      )}

      {message && (
        <pre style={{ background: '#eaffea', padding: 12, marginTop: 20, whiteSpace: 'pre-wrap' }}>
          {message}
        </pre>
      )}
    </div>
  );
}