CREATE TABLE "Faculty" (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255),
    description TEXT
);
CREATE TABLE "User" (
    id SERIAL PRIMARY KEY,
    surname VARCHAR(255),
    name VARCHAR(255),
    patronymic VARCHAR(255),
    role VARCHAR(100),
    login VARCHAR(100),
    password VARCHAR(255)
);
CREATE TABLE "Department" (
    id SERIAL PRIMARY KEY,
    faculty_id INTEGER REFERENCES "Faculty"(id),
    head_id INTEGER REFERENCES "User"(id),
    name VARCHAR(255)
);
CREATE TABLE "Specialty" (
    id SERIAL PRIMARY KEY,
    department_id INTEGER REFERENCES "Department"(id),
    name VARCHAR(255),
    direction VARCHAR(255),
    qualification VARCHAR(255),
    duration INTEGER
);
CREATE TABLE "Discipline" (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255)
);
CREATE TABLE "Curriculum" (
    id SERIAL PRIMARY KEY,
    specialty_id INTEGER REFERENCES "Specialty"(id),
    discipline_id INTEGER REFERENCES "Discipline"(id),
    semester INTEGER,
    certification_type VARCHAR(255)
);
CREATE TABLE "AcademicProgram" (
    id SERIAL PRIMARY KEY,
    specialty_id INTEGER REFERENCES "Specialty"(id),
    discipline_id INTEGER REFERENCES "Discipline"(id),
    name VARCHAR(255),
    start_year INTEGER,
    status VARCHAR(100),
    goals TEXT,
    competencies TEXT,
    requirements TEXT,
    discipline_position TEXT,
    literature TEXT
);
CREATE TABLE "TeacherAssignment" (
    id SERIAL PRIMARY KEY,
    department_id INTEGER REFERENCES "Department"(id),
    teacher_id INTEGER REFERENCES "User"(id)
);
CREATE TABLE "DisciplineTeacher" (
    id SERIAL PRIMARY KEY,
    teacher_id INTEGER REFERENCES "User"(id),
    discipline_id INTEGER REFERENCES "Discipline"(id),
    participation_type VARCHAR(255)
);
CREATE TABLE "WorkLoad" (
    id SERIAL PRIMARY KEY,
    academic_program_id INTEGER REFERENCES "AcademicProgram"(id),
    semester INTEGER,
    lectures INTEGER,
    labs INTEGER,
    self_study INTEGER,
    intermediate_assessment INTEGER,
    assessment_type VARCHAR(255)
);
CREATE TABLE "Sections" (
    id SERIAL PRIMARY KEY,
    work_load_id INTEGER REFERENCES "WorkLoad"(id),
    index INTEGER,
    name VARCHAR(255),
    description TEXT,
    lecture_hours INTEGER,
    lab_hours INTEGER,
    seminar_hours INTEGER,
    self_study_hours INTEGER
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