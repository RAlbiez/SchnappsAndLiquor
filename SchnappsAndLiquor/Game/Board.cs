
namespace SchnappsAndLiquor.Game
{
    public class Board
    {
        public IField[] oFields = new IField[GameParams.MAX_FIELDS];

        public IField this[short pos]
        {
            get => this.oFields[pos];
            set => this.oFields[pos] = value;
        }
    }
}
