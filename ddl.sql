CREATE TABLE "Faculty" (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL UNIQUE,
    description TEXT
);
CREATE TABLE "User" (
    id SERIAL PRIMARY KEY,
    surname VARCHAR(255) NOT NULL,
    name VARCHAR(255) NOT NULL,
    patronymic VARCHAR(255),
    role VARCHAR(100) NOT NULL CHECK (
        role IN ('admin', 'teacher', 'student')
    ),
    login VARCHAR(100) NOT NULL UNIQUE,
    password VARCHAR(255) NOT NULL
);
CREATE TABLE "Department" (
    id SERIAL PRIMARY KEY,
    faculty_id INTEGER NOT NULL REFERENCES "Faculty"(id) ON DELETE CASCADE,
    head_id INTEGER REFERENCES "User"(id) ON DELETE
    SET NULL,
        name VARCHAR(255) NOT NULL,
        CONSTRAINT uq_department_name UNIQUE (name)
);
CREATE TABLE "Specialty" (
    id SERIAL PRIMARY KEY,
    department_id INTEGER NOT NULL REFERENCES "Department"(id) ON DELETE CASCADE,
    name VARCHAR(255) NOT NULL,
    direction VARCHAR(255) NOT NULL,
    qualification VARCHAR(255) NOT NULL,
    duration INTEGER NOT NULL CHECK (duration > 0)
);
CREATE TABLE "Discipline" (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL UNIQUE
);
CREATE TABLE "Curriculum" (
    id SERIAL PRIMARY KEY,
    specialty_id INTEGER NOT NULL REFERENCES "Specialty"(id) ON DELETE CASCADE,
    discipline_id INTEGER NOT NULL REFERENCES "Discipline"(id) ON DELETE CASCADE,
    semester INTEGER NOT NULL CHECK (semester > 0),
    certification_type VARCHAR(255) NOT NULL
);
CREATE TABLE "AcademicProgram" (
    id SERIAL PRIMARY KEY,
    specialty_id INTEGER NOT NULL REFERENCES "Specialty"(id),
    discipline_id INTEGER NOT NULL REFERENCES "Discipline"(id),
    name VARCHAR(255) NOT NULL,
    start_year INTEGER NOT NULL CHECK (start_year >= 2000),
    status VARCHAR(100) NOT NULL DEFAULT 'draft',
    goals TEXT,
    competencies TEXT,
    requirements TEXT,
    discipline_position TEXT,
    literature TEXT
);
CREATE TABLE "TeacherAssignment" (
    id SERIAL PRIMARY KEY,
    department_id INTEGER NOT NULL REFERENCES "Department"(id) ON DELETE CASCADE,
    teacher_id INTEGER NOT NULL REFERENCES "User"(id) ON DELETE CASCADE,
    CONSTRAINT uq_teacher_assignment UNIQUE (department_id, teacher_id)
);
CREATE TABLE "DisciplineTeacher" (
    id SERIAL PRIMARY KEY,
    teacher_id INTEGER NOT NULL REFERENCES "User"(id) ON DELETE CASCADE,
    discipline_id INTEGER NOT NULL REFERENCES "Discipline"(id) ON DELETE CASCADE,
    participation_type VARCHAR(255) NOT NULL
);
CREATE TABLE "WorkLoad" (
    id SERIAL PRIMARY KEY,
    academic_program_id INTEGER NOT NULL REFERENCES "AcademicProgram"(id) ON DELETE CASCADE,
    semester INTEGER NOT NULL CHECK (semester > 0),
    lectures INTEGER NOT NULL DEFAULT 0 CHECK (lectures >= 0),
    labs INTEGER NOT NULL DEFAULT 0 CHECK (labs >= 0),
    self_study INTEGER NOT NULL DEFAULT 0 CHECK (self_study >= 0),
    intermediate_assessment INTEGER NOT NULL DEFAULT 0 CHECK (intermediate_assessment >= 0),
    assessment_type VARCHAR(255) NOT NULL
);
CREATE TABLE "Sections" (
    id SERIAL PRIMARY KEY,
    work_load_id INTEGER NOT NULL REFERENCES "WorkLoad"(id) ON DELETE CASCADE,
    index INTEGER NOT NULL,
    name VARCHAR(255) NOT NULL,
    description TEXT,
    lecture_hours INTEGER NOT NULL DEFAULT 0 CHECK (lecture_hours >= 0),
    lab_hours INTEGER NOT NULL DEFAULT 0 CHECK (lab_hours >= 0),
    seminar_hours INTEGER NOT NULL DEFAULT 0 CHECK (seminar_hours >= 0),
    self_study_hours INTEGER NOT NULL DEFAULT 0 CHECK (self_study_hours >= 0)
);
CREATE INDEX idx_department_faculty_id ON "Department"(faculty_id);
CREATE INDEX idx_department_head_id ON "Department"(head_id);
CREATE INDEX idx_specialty_department_id ON "Specialty"(department_id);
CREATE INDEX idx_curriculum_specialty_id ON "Curriculum"(specialty_id);
CREATE INDEX idx_curriculum_discipline_id ON "Curriculum"(discipline_id);
CREATE INDEX idx_academicprogram_specialty_id ON "AcademicProgram"(specialty_id);
CREATE INDEX idx_academicprogram_discipline_id ON "AcademicProgram"(discipline_id);
CREATE INDEX idx_teacherassignment_department_id ON "TeacherAssignment"(department_id);
CREATE INDEX idx_teacherassignment_teacher_id ON "TeacherAssignment"(teacher_id);
CREATE INDEX idx_disciplineteacher_teacher_id ON "DisciplineTeacher"(teacher_id);
CREATE INDEX idx_disciplineteacher_discipline_id ON "DisciplineTeacher"(discipline_id);
CREATE INDEX idx_workload_academic_program_id ON "WorkLoad"(academic_program_id);
CREATE INDEX idx_sections_work_load_id ON "Sections"(work_load_id);
DROP TABLE IF EXISTS "Sections";
DROP TABLE IF EXISTS "WorkLoad";
DROP TABLE IF EXISTS "DisciplineTeacher";
DROP TABLE IF EXISTS "TeacherAssignment";
DROP TABLE IF EXISTS "AcademicProgram";
DROP TABLE IF EXISTS "Curriculum";
DROP TABLE IF EXISTS "Specialty";
DROP TABLE IF EXISTS "Department";
DROP TABLE IF EXISTS "Discipline";
DROP TABLE IF EXISTS "User";
DROP TABLE IF EXISTS "Faculty";