const API_URL = "http://localhost:5000/api/users";

function goBack() {
    window.location.href = "/users";
}



async function fetchChildDetails() {
    const urlParams = new URLSearchParams(window.location.search);
    const userId = urlParams.get("id");

    const response = await fetch(`${API_URL}/${userId}`);
    const user = await response.json();

    const detailsContainer = document.getElementById("child-details");
    detailsContainer.innerHTML = `
        <p><strong>Имя ребёнка:</strong> ${user.childName}</p>
        <p><strong>Адрес ребёнка:</strong> ${user.childAddress}</p>
        <p><strong>Место учёбы/работы:</strong> ${user.learningWorkplace}</p>
        <p><strong>Контакт ребёнка:</strong> ${user.childContact}</p>
        <p><strong>Имя матери:</strong> ${user.motherName}</p>
        <p><strong>Паспорт матери:</strong> ${user.motherPassport}</p>
        <p><strong>Адрес матери:</strong> ${user.motherAddress}</p>
        <p><strong>Место работы матери:</strong> ${user.motherWorkplace}</p>
        <p><strong>Контакт матери:</strong> ${user.motherContact}</p>
        <p><strong>Имя отца:</strong> ${user.fatherName}</p>
        <p><strong>Паспорт отца:</strong> ${user.fatherPassport}</p>
        <p><strong>Адрес отца:</strong> ${user.fatherAddress}</p>
        <p><strong>Место работы отца:</strong> ${user.fatherWorkplace}</p>
        <p><strong>Контакт отца:</strong> ${user.fatherContact}</p>
        <p><strong>Email:</strong> ${user.email}</p>
        <label for="appointment-date"><strong>Выберите дату:</strong></label>
        <input type="date" id="appointment-date" name="appointment-date">
        <label for="appointment-time"><strong>Выберите время:</strong></label>
        <input type="time" id="appointment-time" name="appointment-time">
        <button onclick="approveUser(${user.id}, '${user.childName}', '${user.email}')">Одобрить</button>

        
    `;
}

async function approveUser(userId, childName, email) {
    const appointmentDate = document.getElementById("appointment-date").value;
    const appointmentTime = document.getElementById("appointment-time").value;

    if (!appointmentDate || !appointmentTime) {
        alert("Пожалуйста, выберите дату и время перед одобрением.");
        return;
    }

    try {
        const response = await fetch(`${API_URL}/${userId}/approve`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                appointmentDate, // дата
                appointmentTime, // время
                childName,       // имя ребенка
                email            // email
            }),
        });

        if (!response.ok) {
            const errorMessage = await response.text();
            throw new Error(`Ошибка сервера: ${errorMessage}`);
        }

        alert("Пользователь успешно одобрен, письмо отправлено!");
        goBack();
    } catch (error) {
        alert(`Ошибка: ${error.message}`);
    }
}



fetchChildDetails();
