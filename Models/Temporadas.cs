using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_de_topicos_Final.Models
{
    public class Temporadas
    {
        public string Titulo { get; set; } = "";
        public string URLimagen { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public byte NumeroTemporada { get; set; }

        public ObservableCollection<Episodios> Episodios { get; set; } = new();
    }
}
