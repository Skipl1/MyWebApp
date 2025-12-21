DELETE FROM "Sections";
DELETE FROM "WorkLoad";
DELETE FROM "DisciplineTeacher";
DELETE FROM "TeacherAssignment";
DELETE FROM "AcademicProgram";
DELETE FROM "Curriculum";
DELETE FROM "Specialty";
DELETE FROM "Department";
DELETE FROM "Discipline";
DELETE FROM "User";
DELETE FROM "Faculty";
TRUNCATE "Sections",
"WorkLoad",
"DisciplineTeacher",
"TeacherAssignment",
"AcademicProgram",
"Curriculum",
"Specialty",
"Department",
"User",
"Discipline",
"Faculty" RESTART IDENTITY CASCADE;
INSERT INTO "Faculty" (name, description)
VALUES (
        'Факультет информационных технологий',
        'Факультет, специализирующийся на IT-направлениях'
    ),
    (
        'Факультет экономики и управления',
        'Факультет, связанный с экономикой и бизнесом'
    ),
    (
        'Факультет инженерии',
        'Факультет, связанный с техническими специальностями'
    ),
    (
        'Юридический факультет',
        'Подготовка специалистов в области юриспруденции'
    ),
    (
        'Факультет дизайна и медиа',
        'Творческие направления и графический дизайн'
    );
INSERT INTO "User" (surname, name, patronymic, role, login, password)
VALUES (
        'Петров',
        'Алексей',
        'Владимирович',
        'admin',
        'admin_1',
        'adminpass1'
    ),
    (
        'Иванова',
        'Марина',
        'Игоревна',
        'admin',
        'admin_2',
        'adminpass2'
    ),
    (
        'Сидоров',
        'Николай',
        'Петрович',
        'teacher',
        'n.sidorov',
        'pass123'
    ),
    (
        'Михайлов',
        'Дмитрий',
        'Сергеевич',
        'teacher',
        'd.mihailov',
        'pass456'
    ),
    (
        'Васильева',
        'Елена',
        'Викторовна',
        'teacher',
        'e.vasilieva',
        'pass789'
    ),
    (
        'Козлов',
        'Артем',
        'Андреевич',
        'teacher',
        'a.kozlov',
        'pass000'
    ),
    (
        'Александров',
        'Игорь',
        'Борисович',
        'teacher',
        'i.alexandrov',
        'pass111'
    ),
    (
        'Морозова',
        'Ольга',
        'Павловна',
        'teacher',
        'o.morozova',
        'pass222'
    ),
    (
        'Белов',
        'Максим',
        'Юрьевич',
        'student',
        'm.belov',
        'stud1'
    ),
    (
        'Чернова',
        'Анна',
        'Денисовна',
        'student',
        'a.chernova',
        'stud2'
    ),
    (
        'Жуков',
        'Павел',
        'Олегович',
        'student',
        'p.zhukov',
        'stud3'
    ),
    (
        'Павлова',
        'Светлана',
        'Ильинична',
        'student',
        's.pavlova',
        'stud4'
    ),
    (
        'Никитин',
        'Егор',
        'Романович',
        'student',
        'e.nikitin',
        'stud5'
    ),
    (
        'Соколов',
        'Виктор',
        'Валерьевич',
        'student',
        'v.sokolov',
        'stud6'
    );
INSERT INTO "Department" (faculty_id, head_id, name)
VALUES (1, 3, 'Кафедра программной инженерии'),
    (1, 4, 'Кафедра системного анализа'),
    (2, 5, 'Кафедра финансов и кредита'),
    (3, 6, 'Кафедра промышленного строительства'),
    (4, 7, 'Кафедра гражданского права'),
    (5, 8, 'Кафедра графического дизайна');
INSERT INTO "Specialty" (
        department_id,
        name,
        direction,
        qualification,
        duration
    )
VALUES (
        1,
        'Программная инженерия',
        '09.03.04',
        'Бакалавр',
        4
    ),
    (
        1,
        'Информационные системы',
        '09.03.02',
        'Бакалавр',
        4
    ),
    (
        2,
        'Прикладная информатика',
        '09.03.03',
        'Бакалавр',
        4
    ),
    (3, 'Экономика', '38.03.01', 'Бакалавр', 4),
    (3, 'Менеджмент', '38.03.02', 'Бакалавр', 4),
    (4, 'Строительство', '08.03.01', 'Бакалавр', 4),
    (5, 'Юриспруденция', '40.03.01', 'Бакалавр', 4),
    (6, 'Дизайн', '54.03.01', 'Бакалавр', 4);
INSERT INTO "Discipline" (name)
VALUES ('Математический анализ'),
    ('Алгебра и геометрия'),
    ('Программирование на Python'),
    ('Объектно-ориентированное программирование'),
    ('Базы данных'),
    ('Макроэкономика'),
    ('Теория государства и права'),
    ('История искусств'),
    ('Сопротивление материалов'),
    ('Маркетинг'),
    ('Архитектура ЭВМ'),
    ('Операционные системы');
INSERT INTO "Curriculum" (
        specialty_id,
        discipline_id,
        semester,
        certification_type
    )
VALUES (1, 1, 1, 'Экзамен'),
    (1, 3, 1, 'Экзамен'),
    (1, 4, 2, 'Экзамен'),
    (1, 5, 3, 'Зачёт'),
    (2, 1, 1, 'Экзамен'),
    (2, 11, 2, 'Экзамен'),
    (3, 1, 1, 'Экзамен'),
    (3, 12, 3, 'Экзамен'),
    (4, 6, 1, 'Экзамен'),
    (4, 10, 2, 'Зачёт'),
    (7, 7, 1, 'Экзамен'),
    (8, 8, 1, 'Зачёт');
INSERT INTO "AcademicProgram" (
        specialty_id,
        discipline_id,
        name,
        start_year,
        status,
        goals,
        competencies,
        requirements,
        discipline_position,
        literature
    )
VALUES (
        1,
        3,
        'Основы Python',
        2025,
        'approved',
        'Освоение синтаксиса Python',
        'ПК-1, ПК-5',
        'Базовая математика',
        '1 курс, 1 семестр',
        'Лутц М., Программирование на Python'
    ),
    (
        1,
        5,
        'Реляционные БД',
        2025,
        'approved',
        'Проектирование SQL баз',
        'ПК-2, ПК-3',
        'Основы алгоритмизации',
        '2 курс, 3 семестр',
        'Дейт К., Введение в системы БД'
    ),
    (
        4,
        6,
        'Общая макроэкономика',
        2025,
        'approved',
        'Анализ рыночных механизмов',
        'ОК-3',
        'Школьный курс истории',
        '1 курс, 1 семестр',
        'Мэнкью Н., Макроэкономика'
    ),
    (
        7,
        7,
        'Правовые основы',
        2025,
        'draft',
        'Изучение структуры права',
        'ОПК-1',
        'Нет',
        '1 курс, 1 семестр',
        'Марченко М., ТГП'
    ),
    (
        6,
        9,
        'Техническая механика',
        2025,
        'approved',
        'Расчет конструкций',
        'ПК-8',
        'Физика',
        '2 курс, 4 семестр',
        'Феодосьев В., Сопромат'
    );
INSERT INTO "WorkLoad" (
        academic_program_id,
        semester,
        lectures,
        labs,
        self_study,
        intermediate_assessment,
        assessment_type
    )
VALUES (1, 1, 32, 32, 64, 4, 'Экзамен'),
    (2, 3, 36, 18, 72, 2, 'Зачёт'),
    (3, 1, 40, 0, 80, 4, 'Экзамен'),
    (4, 1, 44, 0, 88, 4, 'Экзамен'),
    (5, 4, 30, 30, 60, 4, 'Экзамен');
INSERT INTO "Sections" (
        work_load_id,
        index,
        name,
        description,
        lecture_hours,
        lab_hours,
        seminar_hours,
        self_study_hours
    )
VALUES (
        1,
        1,
        'Типы данных',
        'Числа, строки, списки',
        8,
        8,
        4,
        16
    ),
    (
        1,
        2,
        'Функции',
        'Объявление и вызов',
        8,
        8,
        4,
        16
    ),
    (
        2,
        1,
        'SQL запросы',
        'SELECT, JOIN, GROUP BY',
        12,
        6,
        4,
        20
    ),
    (
        3,
        1,
        'ВВП и инфляция',
        'Методы расчета',
        10,
        0,
        10,
        20
    ),
    (
        5,
        1,
        'Растяжение и сжатие',
        'Основные деформации',
        10,
        10,
        5,
        20
    );
INSERT INTO "TeacherAssignment" (department_id, teacher_id)
VALUES (1, 3),
    (1, 4),
    (2, 4),
    (3, 5),
    (4, 6),
    (5, 7),
    (6, 8);
INSERT INTO "DisciplineTeacher" (teacher_id, discipline_id, participation_type)
VALUES (3, 3, 'Лектор'),
    (3, 4, 'Преподаватель'),
    (4, 5, 'Лектор'),
    (4, 11, 'Лектор'),
    (5, 6, 'Лектор'),
    (5, 10, 'Преподаватель'),
    (6, 9, 'Лектор'),
    (7, 7, 'Лектор'),
    (8, 8, 'Лектор');