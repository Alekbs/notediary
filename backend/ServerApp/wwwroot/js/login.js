document.getElementById('loginForm').addEventListener('submit', async function (e) {
    e.preventDefault();
  
    const email = document.getElementById('email').value;
    const password = document.getElementById('password').value;
    const resultDiv = document.getElementById('loginResult');
  
    try {
      const response = await fetch(`${baseUrl}/api/users/login`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email, password })
      });
  
      if (!response.ok) {
        const data = await response.json();
        throw new Error(data.message || 'Ошибка входа');
      }
  
      if (response.ok) {
            const data = await response.json();
            const token = data.token;
            const name = data.name; // допустим, сервер возвращает `fullName`
        
            console.log('Текущий токен:', token);
            localStorage.setItem('authToken', token);
            localStorage.setItem('teacherName', name); // допустим, сервер возвращает `fullName`
        
            window.location.href = 'schedule.html'; // переход на страницу расписания
        }
    
    } catch (error) {
      resultDiv.textContent = error.message;
    }
  });
  