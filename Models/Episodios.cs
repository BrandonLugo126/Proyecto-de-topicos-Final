using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_de_topicos_Final.Models
{
    public class Episodios
    {
        public string Titulo { get; set; } = "";
        public string URLimagen { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public byte NumeroEpisodio { get; set; }
    }
}
