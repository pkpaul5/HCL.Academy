namespace HCL.Academy.Model
{
    public class UserwiseAssessmentsRequest :RequestBase
    {
        public int Userid { get; set; }
        public bool OnlyOnBoardedTraining { get; set; }

    }
}
