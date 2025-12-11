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
    );
INSERT INTO "User" (surname, name, patronymic, role, login, password)
VALUES (
        'Иванов',
        'Иван',
        'Иванович',
        'admin',
        'admin',
        'adminpass'
    ),
    (
        'Петров',
        'Петр',
        'Петрович',
        'teacher',
        'petrov',
        'teacherpass'
    ),
    (
        'Сидорова',
        'Анна',
        'Сергеевна',
        'teacher',
        'sidorova',
        'teacherpass'
    ),
    (
        'Кузнецов',
        'Алексей',
        'Алексеевич',
        'student',
        'kuznetsov',
        'studentpass'
    ),
    (
        'Смирнова',
        'Елена',
        'Дмитриевна',
        'teacher',
        'smirnova',
        'teacherpass'
    );
INSERT INTO "Department" (faculty_id, head_id, name)
VALUES (1, 1, 'Кафедра программной инженерии'),
    (1, 2, 'Кафедра системного анализа'),
    (2, 3, 'Кафедра экономики');
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
        'Информационные системы и технологии',
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
    (3, 'Экономика', '38.03.01', 'Бакалавр', 4);
INSERT INTO "Discipline" (name)
VALUES ('Математический анализ'),
    ('Алгебра и геометрия'),
    ('Программирование'),
    ('Базы данных'),
    ('Экономика предприятия'),
    ('Менеджмент'),
    ('Системный анализ'),
    ('Информационные системы');
INSERT INTO "Curriculum" (
        specialty_id,
        discipline_id,
        semester,
        certification_type
    )
VALUES (1, 1, 1, 'Экзамен'),
    (1, 2, 1, 'Зачёт'),
    (1, 3, 1, 'Экзамен'),
    (1, 4, 2, 'Экзамен'),
    (2, 1, 1, 'Экзамен'),
    (2, 2, 1, 'Зачёт'),
    (2, 3, 2, 'Экзамен'),
    (3, 5, 3, 'Экзамен'),
    (3, 6, 4, 'Зачёт'),
    (4, 5, 1, 'Экзамен'),
    (4, 6, 2, 'Экзамен');
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
        'Программирование 101',
        2025,
        'approved',
        'Цель: обучить основам программирования',
        'Компетенции: Основы программирования',
        'Требования: Знание математики',
        'Место: 2-й семестр',
        'Литература: Кормен, Седжвик'
    ),
    (
        1,
        4,
        'Базы данных',
        2025,
        'draft',
        'Цель: изучить реляционные БД',
        'Компетенции: Работа с SQL',
        'Требования: Программирование',
        'Место: 3-й семестр',
        'Литература: Дейт, Ульман'
    ),
    (
        2,
        7,
        'Системный анализ',
        2025,
        'approved',
        'Цель: обучить моделированию систем',
        'Компетенции: Моделирование процессов',
        'Требования: Программирование',
        'Место: 4-й семестр',
        'Литература: Йодер, Кендэлл'
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
VALUES (1, 2, 32, 16, 64, 0, 'Зачёт'),
    (1, 3, 36, 18, 72, 2, 'Экзамен'),
    (2, 3, 40, 20, 80, 0, 'Экзамен'),
    (3, 4, 44, 22, 88, 2, 'Экзамен');
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
        'Введение в программирование',
        'Основы, типы данных, переменные',
        4,
        2,
        2,
        8
    ),
    (
        1,
        2,
        'Условные операторы',
        'if, else, switch',
        4,
        2,
        2,
        8
    ),
    (
        1,
        3,
        'Циклы',
        'for, while, do-while',
        4,
        2,
        2,
        8
    ),
    (
        2,
        1,
        'Сложные структуры данных',
        'Массивы, списки, деревья',
        6,
        4,
        2,
        12
    ),
    (
        2,
        2,
        'Алгоритмы сортировки',
        'Сортировка выбором, быстрая сортировка',
        6,
        4,
        2,
        12
    ),
    (
        3,
        1,
        'Модель "сущность-связь"',
        'ER-моделирование',
        4,
        2,
        2,
        8
    ),
    (
        3,
        2,
        'Нормализация',
        '1НФ, 2НФ, 3НФ',
        4,
        2,
        2,
        8
    ),
    (
        4,
        1,
        'UML-диаграммы',
        'Диаграммы классов, последовательностей',
        6,
        2,
        2,
        10
    ),
    (
        4,
        2,
        'Моделирование процессов',
        'DFD, IDEF0',
        6,
        2,
        2,
        10
    );
INSERT INTO "TeacherAssignment" (department_id, teacher_id)
VALUES (1, 2),
    (1, 5),
    (2, 3);
INSERT INTO "DisciplineTeacher" (teacher_id, discipline_id, participation_type)
VALUES (2, 3, 'Лектор'),
    (2, 4, 'Лектор'),
    (5, 7, 'Лектор'),
    (3, 8, 'Преподаватель');