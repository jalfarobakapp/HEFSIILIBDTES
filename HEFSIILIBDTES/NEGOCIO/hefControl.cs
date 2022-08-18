using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEFSIILIBDTES.NEGOCIO
{
    /// <summary>
    /// Control de version
    /// </summary>
    internal class hefControl
    {
        internal static HefRespuesta esValido()
        {
            HefRespuesta resp = new HefRespuesta();
            resp.EsCorrecto = true;
            DateTime dt = new DateTime(2099, 04, 17);
            if (DateTime.Now > dt)
            {
                resp.EsCorrecto = false;
                resp.Mensaje = "Validación de producto beta";
                resp.Detalle = "Fin de evaluación";
            }
            return resp;


        }


    }
}
