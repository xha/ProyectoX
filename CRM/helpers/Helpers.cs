

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Pedidos.helpers
{
    
    public static class Helpers
    {
        

        public static MvcHtmlString ActionLinkAllow(this HtmlHelper helper, string Texto, String url, string idTipoUsuario, int Modulo)
        {
            string html = "";
            if (Modulo == 1)
            {
                if(idTipoUsuario == "1" )
                {
                   html = "<li class='nav-item has-treeview '><a href='" + url+"' class='nav-link usuarios'><i class='nav-icon fas fa-users'></i><p>" + Texto + "</p></a></li>";
                }
             
            }

            if (Modulo == 2)
            {
                if (idTipoUsuario == "1"|| idTipoUsuario == "2")
                 {
                     
                      html = "<li class='menu-inventario nav-item has-treeview '><a href='" + url + "' class='nav-link inventario'><i class='nav-icon fas fa-list'></i><p>" + Texto + "<i class='right fas fa-angle-left'></i></p></a><ul class='nav nav-treeview'> <li class='nav-item'> <a href ='/PRODUCTOS/Index' class='nav-link productos'><i class='nav-icon far fa-circle'></i><p>Productos</p></a></li> <li class='nav-item'> <a href = '/CATEGORIAS/Index' class='nav-link categorias'><i class='nav-icon far fa-circle'></i><p>Categorias</p></a></li></ul></li>";
                 }
            }
            if (Modulo == 3)
            {
                if (idTipoUsuario == "1")
                {
                    html = "<li class='nav-item has-treeview menu-pedidos'><a href='" + url + "' class='nav-link pedidos'><i class='nav-icon fas fa-shopping-cart'></i><p>" + Texto + "<i class='right fas fa-angle-left'></i></p></a><ul class='nav nav-treeview'> <li class='nav-item'> <a href ='/Pedidos/Index' class='nav-link listaPedido'><i class='nav-icon far fa-circle'></i><p>Lista pedidos</p></a></li> </ul><ul class='nav nav-treeview'> <li class='nav-item'> <a href ='/Pedidos/Pendientes' class='nav-link pendientes'><i class='nav-icon far fa-circle'></i><p>Pendientes</p></a></li> </ul></li>";
                }
            }
            if (Modulo == 4)
            {
                if (idTipoUsuario == "1")
                {
                    html = "<li class='nav-item has-treeview menu-envios'><a href='" + url + "' class='nav-link envios'><i class='nav-icon fas fa-box-open'></i><p>" + Texto + "<i class='right fas fa-angle-left'></i></p></a><ul class='nav nav-treeview'> <li class='nav-item'> <a href ='/Envios/Almacen' class='nav-link almacen'><i class='nav-icon far fa-circle'></i><p>Almacén</p></a></li> <li class='nav-item'> <a href ='/Envios/Transito' class='nav-link transito'><i class='nav-icon far fa-circle'></i><p>Transito</p></a></li> </ul>"; 

                } 
            }
            if (Modulo == 5)
            {
                if (idTipoUsuario == "1")
                {
                    html = "<li class='nav-item has-treeview '><a  href='" + url + "' class='nav-link camion'><i class='nav-icon fas fa-truck'></i><p>" + Texto + "</p></a></li>";
                }
            }

            if (Modulo == 6)
            {
                if (idTipoUsuario == "1")
                {
                    html = "<li class='nav-item has-treeview '><a href='" + url + "' class='nav-link chofer'><i class='nav-icon fas fa-address-card'></i><p>" + Texto + "</p></a></li>";
                }
            }


                return new MvcHtmlString(html);
            
        }

    }
}