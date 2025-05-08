const dayCarousel = document.getElementById('day-carousel');
const slotsContainer = document.getElementById('slots-container');
const infoPanel = document.getElementById('info-panel');

// ИМЯ залогиненного преподавателя
const loggedTeacher = "Богдан Антон Сергеевич";

// Прогоняем 7 дней вперёд
const today = new Date();
for (let i = 0; i < 7; i++) {
    const day = new Date(today);
    day.setDate(day.getDate() + i);

    const dayElement = document.createElement('div');
    dayElement.className = 'day';
    dayElement.dataset.date = day.toISOString().split('T')[0];
    dayElement.innerHTML = `
        <div>${day.toLocaleDateString('ru-RU', { weekday: 'short' })}</div>
        <div>${day.getDate()}.${day.getMonth() + 1}</div>
    `;
    dayElement.addEventListener('click', () => selectDay(dayElement));
    dayCarousel.appendChild(dayElement);
}

// Генерация фейковых слотов
function generateSlots(date) {
    slotsContainer.innerHTML = '';

    const times = ["13:00", "14:00", "15:00", "16:00", "17:00", "18:00"];
    
    times.forEach((time, index) => {
        const slot = document.createElement('div');
        slot.className = 'slot';

        // имитируем случайное занятие слота
        if (index === 2 || index === 3) {
            slot.classList.add('booked');
        } else if (index === 5) {
            slot.classList.add('own-booking');
        }

        slot.innerText = time;
        slot.dataset.time = time;

        if (!slot.classList.contains('booked') && !slot.classList.contains('own-booking')) {
            slot.addEventListener('click', () => selectSlot(slot, date));
        } else {
            slot.addEventListener('click', () => showSlotInfo(slot, date));
        }

        slotsContainer.appendChild(slot);
    });
}

// Выбор дня
function selectDay(dayElement) {
    document.querySelectorAll('.day').forEach(d => d.classList.remove('active'));
    dayElement.classList.add('active');
    const selectedDate = dayElement.dataset.date;
    generateSlots(selectedDate);
    infoPanel.innerHTML = `<p>Выберите слот...</p>`;
}

// Выбор свободного слота
function selectSlot(slotElement, date) {
    document.querySelectorAll('.slot').forEach(s => s.classList.remove('selected'));
    slotElement.classList.add('selected');

    infoPanel.innerHTML = `
        <h2>Подтверждение слота</h2>
        <p><strong>Время:</strong> ${slotElement.dataset.time}</p>
        <p><strong>Урок:</strong> Ударные инструменты</p>
        <p><strong>Преподаватель:</strong> ${loggedTeacher}</p>
        <p><strong>Ученик:</strong> (свободно)</p>
        <button onclick="confirmSlot('${date}', '${slotElement.dataset.time}')">Подтвердить</button>
    `;
}

// Нажатие на занятый слот
function showSlotInfo(slotElement, date) {
    const teacher = slotElement.classList.contains('own-booking') ? loggedTeacher : "Иванов Петр";
    const student = slotElement.classList.contains('own-booking') ? "Ваш ученик" : "Сидоров Алексей";

    infoPanel.innerHTML = `
        <h2>Информация о слоте</h2>
        <p><strong>Время:</strong> ${slotElement.dataset.time}</p>
        <p><strong>Урок:</strong> Ударные инструменты</p>
        <p><strong>Преподаватель:</strong> ${teacher}</p>
        <p><strong>Ученик:</strong> ${student}</p>
    `;
}

// Подтверждение слота
function confirmSlot(date, time) {
    alert(`Слот ${time} на ${date} подтвержден за вами!`);
    // Здесь потом будет реальный запрос на сервер
}
