document.addEventListener("DOMContentLoaded", async () => {

    const sidebar = document.querySelector(".students-list");
    const subjectElement = document.querySelector('.subject-select');
    
    const token = localStorage.getItem('authToken');
    const teacherName = localStorage.getItem('teacherName');
    const payload = JSON.parse(atob(token.split('.')[1]));
    const userId = payload.UserId;
    const monthSelect = document.getElementById("monthSelect");
    const tableHeader = document.getElementById("tableHeader");
    const weekdaysRow = document.getElementById("weekdaysRow");
  
    const months = [
      'Январь', 'Февраль', 'Март', 'Апрель',
      'Май', 'Июнь', 'Июль', 'Август',
      'Сентябрь', 'Октябрь', 'Ноябрь', 'Декабрь'
    ];
    const weekdays = ['Вс', 'Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб'];
    const year = new Date().getFullYear();
    const dropdown = document.createElement('div');
    dropdown.className = 'dropdown grade-dropdown';
    dropdown.innerHTML = `
    <div class="dropdown-item">5</div>
    <div class="dropdown-item">4</div>
    <div class="dropdown-item">3</div>
    <div class="dropdown-item">2</div>
    `;
document.body.appendChild(dropdown);
    let currentStudents = [];
  
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
  
    if (teacherName) {
      document.querySelector('.teacher-name').textContent = teacherName;
  
      const initials = teacherName.split(' ').map(w => w[0]).join('');
      document.querySelector('.teacher-avatar').textContent = initials;
    }
  
    try {
      const response = await fetch(`${baseUrl}/api/users/students`, {
        method: "GET",
        headers: {
          "Authorization": `Bearer ${token}`,
          "Content-Type": "application/json"
        }
      });
      const students = await response.json();
      currentStudents = students;
      renderStudentRows(currentStudents, document.querySelectorAll("#tableHeader th").length - 1);
    } catch (error) {
      console.error("Ошибка загрузки студентов:", error);
      sidebar.innerHTML += "<p style='color:red;'>Ошибка загрузки</p>";
    }
  
    config.subject.forEach(s => {
      const option = document.createElement("option");
      option.text = s.name;
      option.value = s.subjectId;
      subjectElement.options.add(option);
    });
  
    // Создаём опции месяца
    months.forEach((month, index) => {
      const option = document.createElement("option");
      option.value = index;
      option.textContent = month;
      if (index === new Date().getMonth()) option.selected = true; // по умолчанию выбран текущий месяц
      monthSelect.appendChild(option);
    });
  
    // Функция для обновления дат
    function updateDates(monthIndex) {
      // Очищаем старые данные таблицы
      while (tableHeader.children.length > 1) {
        tableHeader.removeChild(tableHeader.lastChild);
      }
      while (weekdaysRow.children.length > 1) {
        weekdaysRow.removeChild(weekdaysRow.lastChild);
      }
  
      // Создаём новый объект даты
      const date = new Date(year, monthIndex, 1);
      const validDays = [];
  
      // Перебираем все дни месяца и добавляем только рабочие дни (Пн-Пт)
      while (date.getMonth() === monthIndex) {
        const day = date.getDay();
        if (day !== 0 && day !== 6) {
          validDays.push(new Date(date));
  
          // Заголовок для дня
          const thDate = document.createElement("th");
          thDate.className = "border p-2 text-xs w-12 text-center";
          thDate.textContent = date.getDate();
          tableHeader.appendChild(thDate);
  
          // Заголовок для дня недели
          const thWeekday = document.createElement("th");
          thWeekday.className = "p-2 pt-4 text-xs text-gray-500 align-bottom w-12 text-center";
          thWeekday.textContent = weekdays[day];
          weekdaysRow.appendChild(thWeekday);
        }
        date.setDate(date.getDate() + 1);
      }
  
      // Рендерим строки с учениками для текущего месяца
      renderStudentRows(currentStudents, validDays.length);
    }
  
    // Функция для рендеринга строк с учениками
    function renderStudentRows(students, validDatesCount) {
      const tbody = document.querySelector('.students-list');
      tbody.innerHTML = ''; // очистим старые строки
  
      students.forEach(student => {
        const tr = document.createElement('tr');
  
        // Имя ученика в первой ячейке
        const nameTd = document.createElement('td');
        nameTd.textContent = student.name;
        nameTd.className = 'border p-2';
        tr.appendChild(nameTd);
  
        // Добавляем пустые ячейки под каждый учебный день
         // Добавляем пустые ячейки под каждый учебный день
         for (let i = 0; i < validDatesCount; i++) {
            const td = document.createElement('td');
            td.className = 'border p-2 text-center grade-cell';
            
            // В обработчике клика на ячейку заменяем позиционирование:
            td.addEventListener('click', (e) => {
                e.stopPropagation();
                const rect = td.getBoundingClientRect();
                
                // Удаляем подсветку у всех ячеек
                document.querySelectorAll('.focused-cell').forEach(cell => {
                    cell.classList.remove('focused-cell');
                });
                
                // Добавляем подсветку текущей
                td.classList.add('focused-cell');
                
                // Принудительно показываем dropdown для измерения
                dropdown.style.display = 'block';
                const dropdownWidth = dropdown.offsetWidth;
                const dropdownHeight = dropdown.offsetHeight;
                dropdown.style.display = 'none';
                
                // Рассчитываем позицию с учетом границ экрана
                let topPosition = rect.bottom + window.scrollY;
                let leftPosition = rect.right - dropdownWidth + window.scrollX;
                
                // Проверка на выход за правую границу экрана
                if (leftPosition + dropdownWidth > document.documentElement.clientWidth) {
                    leftPosition = document.documentElement.clientWidth - dropdownWidth - 5;
                }
                
                // Проверка на выход за нижнюю границу экрана
                if (topPosition + dropdownHeight > document.documentElement.clientHeight) {
                    topPosition = rect.top - dropdownHeight + window.scrollY - 5;
                }
                
                // Устанавливаем позицию
                dropdown.style.top = `${topPosition}px`;
                dropdown.style.left = `${leftPosition}px`;
                dropdown.style.display = 'block';
                dropdown.currentCell = td;
            });
        
            tr.appendChild(td);
        }
        
        // Добавляем обработчики для dropdown
        dropdown.addEventListener('click', (e) => {
            if (e.target.classList.contains('dropdown-item')) {
                const grade = e.target.textContent;
                if (dropdown.currentCell) {
                    dropdown.currentCell.textContent = grade;
                    dropdown.currentCell.style.backgroundColor = getGradeColor(grade);
                }
                dropdown.style.display = 'none';
                dropdown.currentCell.classList.remove('focused-cell');
            }
        });
        
        // Закрытие dropdown при клике вне
        
        document.addEventListener('click', (e) => {
            if (!e.target.closest('.grade-cell') && !e.target.closest('.dropdown')) {
                dropdown.style.display = 'none';
                // Убираем подсветку
                if (dropdown.currentCell) {
                    dropdown.currentCell.classList.remove('focused-cell');
                }
            }
        });
        
        // Функция для цвета оценки
        function getGradeColor(grade) {
            const colors = {
                '5': '#ccffcc',
                '4': '#ffffcc',
                '3': '#ffe6cc',
                '2': '#ffcccc'
            };
            return colors[grade] || '';
        }
        tbody.appendChild(tr);
      });
    }
  
    // Инициализация таблицы с текущим месяцем
    updateDates(new Date().getMonth());
  
    // При изменении месяца обновляем таблицу
    monthSelect.addEventListener("change", () => {
      updateDates(parseInt(monthSelect.value));
    });
  
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
  });
  