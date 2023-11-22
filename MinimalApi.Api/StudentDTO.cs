using MinimalAPI;

namespace MinimalApi.Api
{
    public class StudentDTO
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Age { get; set; }

        public StudentDTO() { }
        public StudentDTO(Student student) => 
            (Name, Surname, Age) = (student.Name, student.Surname, student.Age);
    }
}
