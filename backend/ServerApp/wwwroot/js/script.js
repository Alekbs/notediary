document.addEventListener('DOMContentLoaded', async function () {
    const token = localStorage.getItem('authToken');
    const teacherName = localStorage.getItem('teacherName');
    const payload = JSON.parse(atob(token.split('.')[1]))
    const userId = payload.UserId;

    let teacherSubjects = [];
    await fetchTeacherSubjects(userId);
    
    const config = {
        teacher: teacherName,
        subject: teacherSubjects,
        startHour: 8,
        endHour: 20,
        lessonDuration: 40,
        breakDuration: 10,
        daysToShow: 7
    };
    console.log('Текущий токен:', token);
    console.log('Имя учителя:', teacherName);
    
    if (teacherName) {
        
        document.querySelector('.teacher-name').textContent = teacherName;

        const initials = teacherName.split(' ').map(w => w[0]).join('');
        document.querySelector('.teacher-avatar').textContent = initials;
    }


    let bookedSlots = [];
    
    const daysContainer = document.querySelector('.days-container');
    const slotDetails = document.querySelector('.slot-details');
    const slotTimeElement = document.querySelector('.slot-time');
    const subjectElement = document.querySelector('.subject');
    const teacherElement = document.querySelector('.teacher');
    const studentElement = document.querySelector('.student');
    const confirmBtn = document.querySelector('.confirm-btn');
    const journalBtn = document.querySelector('.journal-btn');


    async function fetchTeacherSubjects(userId) {
        try {
            const response = await fetch(
                `http://localhost:5000/api/subjects/teacher/${userId}`,
                {
                  method: "GET",
                  headers: {
                    "Authorization": `Bearer ${token}`,
                    "Content-Type": "application/json"
                  }
                }
              );
            const data = await response.json();
            teacherSubjects = data;
        } catch (error) {
            console.error('Ошибка при получении уроков:', error);
        }
        
    }
    async function fetchLessons() {
        const today = new Date();
        const endDate = new Date(today);
        endDate.setDate(today.getDate() + config.daysToShow);

        const startStr = today.toISOString().split('T')[0];
        const endStr = endDate.toISOString().split('T')[0];

        try {
            const response = await fetch(
                `http://localhost:5000/api/calendar/get-lessons?startDate=${startStr}&endDate=${endStr}`,
                {
                  method: "GET",
                  headers: {
                    "Authorization": `Bearer ${token}`,
                    "Content-Type": "application/json"
                  }
                }
              );
            const data = await response.json();
            bookedSlots = data;
        } catch (error) {
            console.error('Ошибка при получении уроков:', error);
        }
    }

    function updateSchedule() {
        // Очистить контейнер с днями
        daysContainer.innerHTML = '';
    
        // Заново получить уроки и сгенерировать расписание
        fetchLessons().then(() => {
            generateDays();
        });
    }
    function findBookedSlot(slotDateTime) {
        return bookedSlots.find(event => {
            if (!event.startTime || !event.endTime) return false;

            const start = new Date(event.startTime);
            const end = new Date(event.endTime);


            return slotDateTime >= start && slotDateTime < end;
        });
    }

    // Функция для парсинга строки в объект
    function parseDescription(descriptionStr) {
        const descriptionObj = {};
        
        // Разделяем строку на части по запятой
        const parts = descriptionStr.split(',').map(part => part.trim());
        
        parts.forEach(part => {
            const [key, value] = part.split(':').map(s => s.trim());
            descriptionObj[key] = value;
        });

        return descriptionObj;
    }
    
    

    function generateTimeSlots() {
        const slots = [];
        let currentTime = config.startHour * 60;

        while (currentTime + config.lessonDuration <= config.endHour * 60) {
            const startHours = Math.floor(currentTime / 60);
            const startMinutes = currentTime % 60;
            const endTime = currentTime + config.lessonDuration;
            const endHours = Math.floor(endTime / 60);
            const endMinutes = endTime % 60;

            const startStr = `${startHours.toString().padStart(2, '0')}:${startMinutes.toString().padStart(2, '0')}`;
            const endStr = `${endHours.toString().padStart(2, '0')}:${endMinutes.toString().padStart(2, '0')}`;

            slots.push({ start: startStr, end: endStr });

            currentTime = endTime + config.breakDuration;
        }

        return slots;
    }

    function generateDays() {
        const timeSlots = generateTimeSlots();
        const today = new Date();
    
        // Получаем локальное время
        const todayLocal = new Date(today.toLocaleString('en-US', { timeZone: 'Europe/Moscow' }));
    
        for (let i = 0; i < config.daysToShow; i++) {
            const date = new Date(todayLocal);
            date.setDate(todayLocal.getDate() + i);
    
            // Используем локальную дату в формате YYYY-MM-DD
            const dateStr = date.toLocaleDateString('en-CA');  // формат YYYY-MM-DD
    
            const dayElement = document.createElement('div');
            dayElement.className = `day ${i === 0 ? 'active' : ''}`;
            dayElement.dataset.date = dateStr;  // Используем правильную дату без времени
            dayElement.style.minWidth = '220px';
    
            const dateStrFormatted = date.toLocaleDateString('ru-RU', {
                day: 'numeric',
                month: 'long'
            });
    
            dayElement.innerHTML = `
                <div class="date">${dateStrFormatted}</div>
                <div class="day-slots"></div>
            `;
    
            const slotsContainer = dayElement.querySelector('.day-slots');
    
            timeSlots.forEach((time, index) => {
                const slot = document.createElement('div');
                slot.className = 'slot';
                slot.dataset.time = `${time.start} - ${time.end}`;
                slot.textContent = `${time.start} - ${time.end}`;
    
                // Создаем слот с учетом локального времени
                const slotDateTime = new Date(`${dateStr}T${time.start}:00`);
    
                // Дополнительное логирование для проверки
    
                const booked = findBookedSlot(slotDateTime);
    
                if (booked) {
                    descriptionObj = parseDescription(booked.description);
                    console.log(`teacher: ${descriptionObj.teacher}`);
                    slot.classList.add('booked');
                    if (booked.description && descriptionObj.teacher.includes(config.teacher)) {
                        slot.classList.add('owned');
                        console.log(`[Owned] ${slotDateTime.toLocaleString()} - слот занят`);
                    }
                } else {
                }
    
                slot.addEventListener('click', handleSlotClick);
                slotsContainer.appendChild(slot);
            });
    
            daysContainer.appendChild(dayElement);
        }
    }
    
    

    function handleSlotClick() {
        if (this.classList.contains('booked') && !this.classList.contains('owned')) return;

        document.querySelector('.day.active')?.classList.remove('active');
        const selectedDay = this.closest('.day');
        selectedDay.classList.add('active');

        document.querySelectorAll('.slot.selected').forEach(s => s.classList.remove('selected'));
        this.classList.add('selected');

        updateSlotDetails(this);
        slotDetails.classList.remove('hidden');
    }

    function updateSlotDetails(slot) {
        slotTimeElement.textContent = slot.dataset.time;
        config.subject.forEach(s => {
            const option = document.createElement("option");
            option.text = s.name;
            option.value = s.subjectId;

            subjectElement.options.add(option);
        });

        teacherElement.textContent = config.teacher;

        if (slot.classList.contains('booked')) {
            studentElement.textContent = 'Занято';
            confirmBtn.style.display = 'none';
        } else {
            studentElement.textContent = '-';
            confirmBtn.style.display = 'block';
        }
    }

    journalBtn.addEventListener('click', function () {
        window.location.href = 'main.html'; // переход на страницу расписания
    });

    confirmBtn.addEventListener('click', function () {
        const selectedSlot = document.querySelector('.slot.selected');

        if (selectedSlot) {
            const selectedDay = document.querySelector('.day.active');
            const selectedDate = selectedDay.dataset.date;
            const selectedTime = selectedSlot.dataset.time;
            const startTime = selectedTime.split(' - ')[0];

            const fullStartTime = `${selectedDate}T${startTime}:00`;

            const lessonData = {
                StartTime: fullStartTime,
                Summary: config.subject,
                Teacher: config.teacher,
                Student: 'Юрий Соколов'
            };

            const token = localStorage.getItem('authToken');

            fetch('http://localhost:5000/api/calendar/add-lesson', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify(lessonData)
            })

            .then(response => response.json())
            .then(data => {
                console.log('Урок добавлен:', data);
                updateSchedule();
            })
            .catch(error => {
                console.error('Ошибка:', error);
            });
        }
    });

    function initCarousel() {
        const prevBtn = document.querySelector('.carousel-btn.prev');
        const nextBtn = document.querySelector('.carousel-btn.next');

        let currentScroll = 0;
        const scrollAmount = 270;

        prevBtn.addEventListener('click', function () {
            currentScroll = Math.max(currentScroll - scrollAmount, 0);
            daysContainer.scrollTo({
                left: currentScroll,
                behavior: 'smooth'
            });
        });

        nextBtn.addEventListener('click', function () {
            currentScroll = Math.min(
                currentScroll + scrollAmount,
                daysContainer.scrollWidth - daysContainer.clientWidth
            );
            daysContainer.scrollTo({
                left: currentScroll,
                behavior: 'smooth'
            });
        });
    }
    
    await fetchLessons();
    generateDays();
    initCarousel();
});
