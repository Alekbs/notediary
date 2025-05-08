const API_URL = "http://localhost:5000/api/users";

function goToUsers() {
    window.location.href = "/users";
}

function goToSign() {
    window.location.href = "/sign";
}

async function fetchUsers() {
    const response = await fetch(API_URL);
    const users = await response.json();

    console.log(users); // Логирование данных для проверки структуры


    const usersList = document.getElementById("users-list");
    usersList.innerHTML = users
        .filter(user => !user.IsApproved) // Фильтруем только детей
        .map(
            user => `
        <tr>
            <td>
                <a href="/wwwroot/pages/user-details.html?id=${user.id}">${user.childName}</a>
            </td>
        </tr>`
        )
        .join("");
}

// Переход на страницу пользователя
if (window.location.pathname.includes("users")) {
    fetchUsers();
}

async function approveUser(userId) {
    await fetch(`${API_URL}/${userId}/approve`, { method: "POST" });
    fetchUsers();
}

document.getElementById("sign-form")?.addEventListener("submit", async e => {
    e.preventDefault();
    const name = document.getElementById("name").value;
    const email = document.getElementById("email").value;

    await fetch(API_URL, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ name, email }),
    });

    alert("User added!");
    goToUsers();
});

if (window.location.pathname.includes("users")) {
    fetchUsers();
}


// Пример: получить занятия на дату
async function getLessons(date) {
    try {
        const response = await fetch(`/api/calendar/get-lessons?date=${encodeURIComponent(date)}`);
        const lessons = await response.json();
        console.log(lessons); // Массив занятий
        return lessons;
    } catch (error) {
        console.error('Ошибка при получении занятий:', error);
    }
}

// Пример: добавить новое занятие
async function addLesson(summary, startTime) {
    try {
        const response = await fetch('/api/calendar/add-lesson', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ summary, startTime })
        });

        if (response.ok) {
            console.log('Занятие добавлено!');
        } else {
            const err = await response.json();
            console.error('Ошибка при добавлении занятия:', err.error);
        }
    } catch (error) {
        console.error('Ошибка при добавлении занятия:', error);
    }
}

// Пример: обновить существующее занятие
async function updateLesson(eventId, newSummary, newStartTime) {
    try {
        const response = await fetch('/api/calendar/update-lesson', {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ eventId, newSummary, newStartTime })
        });

        if (response.ok) {
            console.log('Занятие обновлено!');
        } else {
            const err = await response.json();
            console.error('Ошибка при обновлении занятия:', err.error);
        }
    } catch (error) {
        console.error('Ошибка при обновлении занятия:', error);
    }
}

// Пример: удалить занятие
async function deleteLesson(eventId) {
    try {
        const response = await fetch(`/api/calendar/delete-lesson/${encodeURIComponent(eventId)}`, {
            method: 'DELETE'
        });

        if (response.ok) {
            console.log('Занятие удалено!');
        } else {
            const err = await response.json();
            console.error('Ошибка при удалении занятия:', err.error);
        }
    } catch (error) {
        console.error('Ошибка при удалении занятия:', error);
    }
}

