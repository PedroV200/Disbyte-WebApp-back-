﻿namespace WebApiSample.Models
{
    public class Usuario
    {
        public int userid{get;set;}
        public string username{get;set;}
        public string email{get;set;}
        public string password{get;set;}
        public string role{get;set;}
        public int paisregion_id{get;set;}
        
    }

    public class UsuarioVista
    {
        public int userid{get;set;}
        public string username{get;set;}
        public string email{get;set;}
        public string password{get;set;}
        public string role{get;set;}
        public int paisregion_id{get;set;}
        public string pais {get;set;}
        public string region {get;set;}
        
    }

    public class OwnPresup
    {
        public int id{get;set;}
        public string own{get;set;}
    }
}
