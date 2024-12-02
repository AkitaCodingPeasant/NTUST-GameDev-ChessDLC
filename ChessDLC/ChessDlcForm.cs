using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessDLC {
    public partial class ChessDlcForm : Form {
        public ChessDlcForm() {
            InitializeComponent();
            ChessBoard.Initialize();
            ChessBoard.BuildForm(this);
            ChessBoard.GenerateChessBoard();
        }
    }
}
