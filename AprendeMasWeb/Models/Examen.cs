namespace AprendeMasWeb.Models
{
	public class Examen
	{
		public int ExamenId { get; set; }
		public string NombreExam { get; set; }
		public int RubricaId { get; set; }
		public RubricaEvaluacion Rubrica { get; set; }
	}
}
