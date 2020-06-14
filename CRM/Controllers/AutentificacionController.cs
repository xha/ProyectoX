using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Configuration;
using CRM.sp_estatico;
using CRM.Models;
using CRM.sp_dinamic;
using CRM.Conexion;
using Newtonsoft.Json.Linq;

namespace CRM.Controllers
{
    public class AutentificacionController : Controller
    {
        private conexion CD = new conexion();

        // GET: Autentificacion
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(User model)
        {
            CLIENTES_PORTALESEntities bd = new CLIENTES_PORTALESEntities(CD.ConexFIJA(WebConfigurationManager.AppSettings["datasour"].ToString(), WebConfigurationManager.AppSettings["catalog"].ToString(), WebConfigurationManager.AppSettings["user"].ToString(), WebConfigurationManager.AppSettings["password"].ToString()));
            if (ModelState.IsValid)
            { 
                try
                {
                    bool LicenciaValida;
                    //LicenciaValida = CD.ConsultaPost("ValidarLicencia", model.RUC, "03");
                    //OJO
                    LicenciaValida = true;
                    if (LicenciaValida)
                    {
                        /*bd.Database.ExecuteSqlCommand("IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('dbo.SP_CLIENTE_LOGIN'))   EXEC('CREATE PROCEDURE SP_CLIENTE_LOGIN (  @ruc NVARCHAR(100)  )    AS    BEGIN   SELECT* FROM[CLIENTES_PORTALES].DBO.Clientes WHERE RUC_CLIENTE = @ruc; END');");
                        //SP_CLIENTE_LOGIN_Result dataBase = bd.SP_CLIENTE_LOGIN(model.Encriptar(model.RUC)).FirstOrDefault();
                        if (dataBase != null)
                        {*/
                            /*Session["ruc"] = model.Desencriptar(dataBase.RUC_Cliente);
                            Session["datasour"] = model.Desencriptar(dataBase.Servidor);
                            Session["catalog"] = "CRM";
                            Session["user"] = model.Desencriptar(dataBase.Usuario_Server);
                            Session["password"] = model.Desencriptar(dataBase.Contrasenia_Server);*/
                            Session["ruc"] = model.RUC;
                            Session["datasour"] = WebConfigurationManager.AppSettings["datasour"].ToString();
                            Session["catalog"] = "CRM";
                            Session["user"] = WebConfigurationManager.AppSettings["user"].ToString();
                            Session["password"] = WebConfigurationManager.AppSettings["password"].ToString();
                            BDWENCOEntities BD = new BDWENCOEntities(CD.ConexDinamica(Session["datasour"].ToString(), Session["catalog"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
                            //BD.Database.ExecuteSqlCommand(" IF NOT EXISTS(SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('dbo.SP_PORTAL_VERIFICAR_DATABASE')) EXEC('    CREATE PROCEDURE SP_PORTAL_VERIFICAR_DATABASE  @BASEDATOS VARCHAR(10) AS IF   NOT EXISTS (SELECT NAME FROM MASTER.DBO.SYSDATABASES WHERE NAME = @BASEDATOS) BEGIN 	DECLARE @ANEXO VARCHAR(13);  	set @ANEXO = ''0''; 		 END ELSE 	  BEGIN    set @ANEXO = ''1'';  END select @anexo as data')");
                            BD.Database.ExecuteSqlCommand("IF   NOT EXISTS (SELECT * FROM sysobjects WHERE NAME='CRM_Usuarios_Rol'  and TYPE='U') BEGIN CREATE TABLE CRM_Usuarios_Rol (ID BIGINT PRIMARY KEY IDENTITY(1, 1) NOT NULL,codigoEmpresa varchar(6),alias VARCHAR(20), nombreUser VARCHAR(20),contrase VARCHAR(20),email varchar(50),Rol VARCHAR(50));    INSERT INTO CRM_Usuarios_Rol(codigoEmpresa, alias, nombreUser, contrase, email, Rol) VALUES('001', 'admin', 'admin', ',', '', 'ADMINISTRADOR'); END");
                            BD.Database.ExecuteSqlCommand("IF   NOT EXISTS (SELECT * FROM sysobjects WHERE NAME='EMPRESA'  and TYPE='U') BEGIN CREATE TABLE EMPRESA(EMP_CODIGO varchar(6) NULL,TIPODOC_CODIGO varchar(2) NULL,EMP_RAZON_NOMBRE varchar(100) NULL,EMP_APELLIDOPAT varchar(50) NULL,EMP_APELLIDOMAT varchar(50) NULL,EMP_DIRECCION varchar(50) NULL,EMP_TELEFONO varchar(15) NULL,EMP_FAX varchar(15) NULL,EMP_RUC_DOCUMENTO varchar(11) NULL,EMP_REPRESENTANTE varchar(40) NULL,EMP_DOCUMENTO varchar(15) NULL,EMP_NIVEL smallint NULL,EMP_NIV01 smallint NULL,EMP_NIV02 smallint NULL,EMP_NIV03 smallint NULL,EMP_NIV04 smallint NULL,EMP_NIV05 smallint NULL,EMP_NIV06 smallint NULL,EMP_PANTALLA varchar(100) NULL,EMP_NUMEAUTO bit NOT NULL default 0,EMP_NUMECARAC numeric(18, 0) NULL,EMP_REPORTE varchar(100) NULL,EMP_ACTIVA bit NOT NULL default 0,EMP_NIV07 smallint NULL,EMP_NIV08 smallint NULL,EMP_PLANILLAS varchar(50) NULL,ENVIA_ALERTA bit NULL,LOGO image NULL,EMP_PORTAL_PROVEEDOR bit NULL default 0,EMP_PORTAL_RENDICIONCTA bit NULL);    INSERT INTO EMPRESA(EMP_CODIGO,EMP_RAZON_NOMBRE,EMP_PANTALLA) VALUES ('001','DEMOSTRACION','DEMOSTRACION'); END");
                            BD.Database.ExecuteSqlCommand("IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('dbo.SP_EMPRESA_CLIENTE_CRM')) EXEC('   CREATE PROCEDURE SP_EMPRESA_CLIENTE_CRM AS    BEGIN    SELECT* FROM[BDWENCO].DBO.USUARIO_FAC; END'); IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('dbo.SP_EMPRESA_USUARIO_CRM')) EXEC('   CREATE PROCEDURE SP_EMPRESA_USUARIO_CRM AS    BEGIN    SELECT* FROM EMPRESA; END');");
                            string CONTRA = model.CODIFICA(model.Password, 5);

                            CRM_Usuarios_Rol usuario_rol = BD.CRM_Usuarios_Rol.Where(b => b.contrase == CONTRA).Where(b => b.alias.ToUpper() == model.Usuario.ToUpper()).FirstOrDefault();
                            if (usuario_rol != null)
                            {
                                //string codigo_momentaeio = BD.SP_EMPRESA_USUARIO_CRM().Where(b => b.EMP_RUC_DOCUMENTO == Session["ruc"].ToString()).First().EMP_CODIGO;
                                string codigo_momentaeio = usuario_rol.codigoEmpresa;

                                Session["catalog_user"] = "CRM";

                                /*SP_PORTAL_VERIFICAR_DATABASE_Result validacion = BD.SP_PORTAL_VERIFICAR_DATABASE1(usuario_rol.codigoEmpresa + "BDCOMUN").First();
                                if (validacion.data == "0")
                                {
                                    ViewBag.Error = "La empresa " + usuario_rol.codigoEmpresa + " asociada al usuario no tiene el modulo de ventas activado";
                                    return View(model);
                                }*/
                                Verificar_Data_User_Admin(usuario_rol);
                                SP_EMPRESA_USUARIO_CRM_Result empresa = BD.SP_EMPRESA_USUARIO_CRM().Where(x => x.EMP_CODIGO == usuario_rol.codigoEmpresa).FirstOrDefault();
                                Session["codigo"] = empresa.EMP_CODIGO;
                                Session["empresa"] = empresa;
                                Session["rol"] = usuario_rol.Rol;
                                Session["alias"] = usuario_rol.alias;
                                model.rol = usuario_rol.Rol;

                                if (usuario_rol.Rol == "ADMINISTRADOR")
                                {
                                    Session["identificador"] = "CRM";
                                    model.tipo = "CRM";
                                    Session["nombre_admin"] = usuario_rol.nombreUser;
                                    Session["cliente"] = model;
                                    return RedirectToAction("Index", "Administrador");

                                }
                                else
                                {
                                    Session["Nombreuser"] = usuario_rol.nombreUser;
                                    Session["user_logueado"] = usuario_rol;
                                    model.tipo = "CRM_V";
                                    Session["cliente"] = model;
                                    return RedirectToAction("Index", "Flujo");
                                }

                            }
                            else
                            {
                                ADMINISTRADOR admin_rp = BD.ADMINISTRADOR.Where(b => b.ADM_NOMBRE.ToUpper() == model.Usuario.ToUpper()).Where(b => b.ADM_PASSWORD == CONTRA).FirstOrDefault();
                                if (admin_rp != null)
                                {
                                    Verificar_Data();
                                    if (Session["EMPRESAS"] == null)
                                    {
                                        ViewBag.Error = "No existen empresas configuradas para el modulo de Ventas";
                                        return View();
                                    }
                                    Session["rol"] = "ADMINISTRADOR";
                                    model.rol = "ADMINISTRADOR";
                                    model.tipo = "RP";
                                    Session["identificador"] = "RP";
                                    Session["nombre_admin"] = admin_rp.ADM_NOMBRE;
                                    model.rol = Session["rol"].ToString();
                                    Session["cliente"] = model;

                                    return RedirectToAction("Usuarios", "RP");
                                }
                                else
                                {
                                    ViewBag.Error = "El usuario o la contraseña son incorrectos";
                                    return View(model);
                                }

                            }

                        //}
                    } 
                    else
                    {
                        ViewBag.Error = "Licencia se encuentra vencida";
                        return View(model);
                    }
                }
                catch (Exception e)
                {
                    ViewBag.Error = e.Message;
                    return View(model);
                }
            }
            return View(model);
        }



        public void Verificar_Data() {
            BDWENCOEntities BD = new BDWENCOEntities(CD.ConexDinamica(Session["datasour"].ToString(), Session["catalog"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
            
            var empresas_asociadas = new List<SP_EMPRESA_USUARIO_CRM_Result>();
            var empresas = new List<SP_EMPRESA_USUARIO_CRM_Result>();

            empresas_asociadas = BD.SP_EMPRESA_USUARIO_CRM().ToList();
            foreach (var i in empresas_asociadas)
            {
                string base_datos = "CRM";
                /*SP_PORTAL_VERIFICAR_DATABASE_Result validacion = BD.SP_PORTAL_VERIFICAR_DATABASE1(base_datos).First();
                if (validacion.data != "0")
                {*/
                    BD.Database.ExecuteSqlCommand("IF   NOT EXISTS (SELECT * FROM [" + base_datos + "].[dbo].sysobjects WHERE NAME='TIPO_MONEDA'  and TYPE='U')  BEGIN CREATE TABLE[" + base_datos + "].[dbo].[TIPO_MONEDA]([TIPOMON_CODIGO] [varchar](2) PRIMARY KEY,[TIPOMON_DESCRIPCION][varchar](20) NULL,[TIPOMON_NACIONALIDAD][bit] NOT NULL default 0,[TIPOMON_SIMBOLO][varchar](2) NULL); INSERT INTO TIPO_MONEDA(TIPOMON_CODIGO,TIPOMON_DESCRIPCION,TIPOMON_NACIONALIDAD,TIPOMON_SIMBOLO) VALUES('ME','DOLARES',0,'$'); INSERT INTO TIPO_MONEDA(TIPOMON_CODIGO,TIPOMON_DESCRIPCION,TIPOMON_NACIONALIDAD,TIPOMON_SIMBOLO) VALUES('MN','BOLIVARES',1,'Bs'); END ");
                    BD.Database.ExecuteSqlCommand("IF   NOT EXISTS (SELECT * FROM [" + base_datos + "].[dbo].sysobjects WHERE NAME='GRUPOS'  and TYPE='U')  BEGIN CREATE TABLE[" + base_datos + "].[dbo].[GRUPOS]( [id_grupo][int] NOT NULL, [descripcion][varchar](50) NULL,CONSTRAINT[PK_GRUPOS] PRIMARY KEY CLUSTERED([id_grupo] ASC )WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]) ON[PRIMARY] END ");
                    BD.Database.ExecuteSqlCommand("IF   NOT EXISTS(SELECT * FROM[" + base_datos + "].[dbo].sysobjects WHERE NAME = 'GRUPO_USUARIO'  and TYPE = 'U') BEGIN CREATE TABLE[" + base_datos + "].[dbo].[GRUPO_USUARIO](    [id_grupoUuario][int] NOT NULL,    [id_grupo][int] NULL,    [int_usuario][int] NULL, CONSTRAINT[PK_GRUPO_USUARIO] PRIMARY KEY CLUSTERED (   [id_grupoUuario] ASC )WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY] ) ON[PRIMARY] ALTER TABLE[" + base_datos + "].[dbo].[GRUPO_USUARIO]  WITH CHECK ADD CONSTRAINT[FK_GRUPO_USUARIO_GRUPOS] FOREIGN KEY([id_grupo]) REFERENCES[" + base_datos + "].[dbo].[GRUPOS] ([id_grupo]) ALTER TABLE[" + base_datos + "].[dbo].[GRUPO_USUARIO] CHECK CONSTRAINT[FK_GRUPO_USUARIO_GRUPOS] END     ");
                    BD.Database.ExecuteSqlCommand("IF   NOT EXISTS(SELECT * FROM[" + base_datos + "].[dbo].sysobjects WHERE NAME = 'TIPO_ACTIVIDAD'  and TYPE = 'U') BEGIN CREATE TABLE [" + base_datos + "].[dbo].[TIPO_ACTIVIDAD]( [id_tipoActividad][int] IDENTITY(1, 1) NOT NULL, [descripcion][nvarchar](100) NULL, [estado][bit] NOT NULL, [color][nvarchar](20) NOT NULL, PRIMARY KEY CLUSTERED ( [id_tipoActividad] ASC )WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY] ) ON[PRIMARY] SET IDENTITY_INSERT[" + base_datos + "].[dbo].[TIPO_ACTIVIDAD] ON INSERT[" + base_datos + "].[dbo].[TIPO_ACTIVIDAD] ([id_tipoActividad], [descripcion], [estado], [color]) VALUES(1, N'Llamada', 1, N'#0800ff') INSERT[" + base_datos + "].[dbo].[TIPO_ACTIVIDAD] ([id_tipoActividad], [descripcion], [estado], [color]) VALUES(2, N'Correo Electrónico', 1, N'#00ff1d') INSERT[" + base_datos + "].[dbo].[TIPO_ACTIVIDAD] ([id_tipoActividad], [descripcion], [estado], [color]) VALUES(3, N'Visita Personal', 1, N'#ffbb00') INSERT[" + base_datos + "].[dbo].[TIPO_ACTIVIDAD] ([id_tipoActividad], [descripcion], [estado], [color]) VALUES(4, N'Reunión Digital', 1, N'#a100ff') SET IDENTITY_INSERT[" + base_datos + "].[dbo].[TIPO_ACTIVIDAD] OFF SET ANSI_PADDING ON ALTER TABLE[" + base_datos + "].[dbo].[TIPO_ACTIVIDAD] ADD CONSTRAINT[uq_ta_descripcion] UNIQUE NONCLUSTERED ( [descripcion] ASC )WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY] ALTER TABLE[" + base_datos + "].[dbo].[TIPO_ACTIVIDAD]  ADD DEFAULT((1)) FOR[estado] ALTER TABLE[" + base_datos + "].[dbo].[TIPO_ACTIVIDAD]ADD DEFAULT('#FFF') FOR[color] END ");
                    BD.Database.ExecuteSqlCommand("IF   NOT EXISTS (SELECT * FROM [" + base_datos + "].[dbo].sysobjects WHERE NAME='POTENCIALCLI'  and TYPE='U') BEGIN CREATE TABLE [" + base_datos + "].[dbo].[POTENCIALCLI](	[CCODCLI] [varchar](11) NOT NULL,	[CNOMCLI] [varchar](100) NULL,	[CDIRCLI] [varchar](100) NULL,	[CTELEFO] [varchar](30) NULL,	[CNUMRUC] [varchar](11) NULL,	[CDOCIDEN] [varchar](15) NULL,	[CFLADES] [varchar](1) NULL,	[NPORDES] [numeric](15, 6) NULL,	[CTIPVTA] [varchar](4) NULL,	[CESTADO] [varchar](1) NULL,	[DFECINS] [smalldatetime] NULL,	[CNOMREP] [varchar](30) NULL,	[CDISTRI] [varchar](8) NULL,	[CUSUARI] [varchar](8) NULL,	[DFECCRE] [smalldatetime] NULL,	[DFECMOD] [smalldatetime] NULL,	[CTIPPRE] [varchar](4) NULL,	[CVENDE] [varchar](2) NULL,	[CZONVTA] [varchar](8) NULL,	[CPAIS] [varchar](30) NULL,	[CDEPT] [varchar](30) NULL,	[CPROV] [varchar](30) NULL,	[DIRENT] [varchar](100) NULL,	[LOCENT] [varchar](3) NULL,	[LOCCLI] [varchar](3) NULL,	[LOCEST] [varchar](2) NULL,	[ZONVTA] [varchar](5) NULL,	[DIAATE] [varchar](1) NULL,	[SITCLI] [varchar](2) NULL,	[TIPIDE] [varchar](2) NULL,	[REGVTA] [varchar](7) NULL,	[MONCRE] [varchar](2) NULL,	[LMCRUS] [numeric](15, 6) NULL,	[LMCRMN] [numeric](15, 6) NULL,	[SALDMN] [numeric](18, 6) NULL,	[SALDUS] [numeric](18, 6) NULL,	[LIMFAC] [numeric](15, 6) NULL,	[TOTFAC] [numeric](15, 6) NULL,	[TOTLEP] [numeric](15, 6) NULL,	[TOTCHD] [numeric](15, 6) NULL,	[FULABO] [varchar](6) NULL,	[FECUFA] [varchar](6) NULL,	[NROSOL] [varchar](6) NULL,	[OBSERV] [varchar](30) NULL,	[DOCVEN] [numeric](15, 6) NULL,	[DIAVEN] [numeric](15, 6) NULL,	[NROORD] [varchar](10) NULL,	[MORLET] [numeric](15, 6) NULL,	[MORFAC] [numeric](15, 6) NULL,	[CHEDEV] [numeric](15, 6) NULL,	[LETPRO] [numeric](15, 6) NULL,	[CTIPCLI] [varchar](2) NULL,	[CGIRNEG] [varchar](2) NULL,	[CTERRIT] [varchar](2) NULL,	[CRUTA] [varchar](2) NULL,	[CSEGMEN] [varchar](3) NULL,	[CUBISEG] [varchar](3) NULL,	[CCODBAN] [varchar](3) NULL,	[CNUMCTA] [varchar](16) NULL,	[CFREVIS] [varchar](8) NULL,	[CHORATE] [varchar](20) NULL,	[CTIPATE] [varchar](2) NULL,	[CNUMFAX] [varchar](15) NULL,	[CEMAIL] [varchar](200) NULL,	[CHOST] [varchar](80) NULL,	[CZONPOS] [varchar](5) NULL,	[COMENTA] [varchar](500) NULL,	[RETEN] [bit] NULL,	[CFLAGPRIN] [bit] NOT NULL,	[CCODTRANS] [varchar](11) NOT NULL,	[SIN_CONTROL_LIMCREDITO] [bit] NULL,	[SUBCLA01] [varchar](30) NULL,	[SUBCLA02] [varchar](30) NULL,	[ZON_CODIGO] [varchar](20) NULL,	[CTIPO_DOCUMENTO] [varchar](2) NULL,	[CAPELLIDO_PATERNO] [varchar](20) NULL,	[CAPELLIDO_MATERNO] [varchar](20) NULL,	[CPRIMER_NOMBRE] [varchar](20) NULL,	[CSEGUNDO_NOMBRE] [varchar](20) NULL,	[TCL_CODIGO] [varchar](3) NOT NULL,	[NRO_AUTORIZACION_DIGEMID] [varchar](50) NULL,	[CONTACTO_COBRANZA] [varchar](40) NOT NULL,	[CCODCLAS] [varchar](8) NOT NULL,	[FLGPORTAL_CLIENTES] [bit] NULL,	[ULTIMO_TC_VTA] [numeric](18, 6) NULL,	[DESCUENTO_ESP] [numeric](18, 6) NULL,	[CEMAIL_CONTACTO] [varchar](120) NULL,	[UBIGEO] [varchar](12) NULL,	[FEC_INACTIVO_BLOQUEADO] [datetime] NULL,	[COD_AUDITORIA] [varchar](12) NULL, CONSTRAINT [PK_POTENCIALCLI] PRIMARY KEY CLUSTERED ([CCODCLI] ASC )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY] ALTER TABLE [" + base_datos + "].[dbo].[POTENCIALCLI] ADD  DEFAULT ((0)) FOR [NPORDES] ALTER TABLE [" + base_datos + "].[dbo].[POTENCIALCLI] ADD  DEFAULT ((0)) FOR [LMCRUS] ALTER TABLE [" + base_datos + "].[dbo].[POTENCIALCLI] ADD  DEFAULT ((0)) FOR [LMCRMN] ALTER TABLE [" + base_datos + "].[dbo].[POTENCIALCLI] ADD  DEFAULT ((0)) FOR [SALDMN] ALTER TABLE [" + base_datos + "].[dbo].[POTENCIALCLI] ADD  DEFAULT ((0)) FOR [SALDUS] ALTER TABLE [" + base_datos + "].[dbo].[POTENCIALCLI] ADD  DEFAULT ((0)) FOR [LIMFAC] ALTER TABLE [" + base_datos + "].[dbo].[POTENCIALCLI] ADD  DEFAULT ((0)) FOR [TOTFAC] ALTER TABLE [" + base_datos + "].[dbo].[POTENCIALCLI] ADD  DEFAULT ((0)) FOR [TOTLEP] ALTER TABLE [" + base_datos + "].[dbo].[POTENCIALCLI] ADD  DEFAULT ((0)) FOR [TOTCHD] ALTER TABLE [" + base_datos + "].[dbo].[POTENCIALCLI] ADD  DEFAULT ((0)) FOR [DOCVEN] ALTER TABLE [" + base_datos + "].[dbo].[POTENCIALCLI] ADD  DEFAULT ((0)) FOR [DIAVEN] ALTER TABLE [" + base_datos + "].[dbo].[POTENCIALCLI] ADD  DEFAULT ((0)) FOR [MORLET] ALTER TABLE [" + base_datos + "].[dbo].[POTENCIALCLI] ADD  DEFAULT ((0)) FOR [MORFAC] ALTER TABLE [" + base_datos + "].[dbo].[POTENCIALCLI] ADD  DEFAULT ((0)) FOR [CHEDEV] ALTER TABLE [" + base_datos + "].[dbo].[POTENCIALCLI] ADD  DEFAULT ((0)) FOR [LETPRO] ALTER TABLE [" + base_datos + "].[dbo].[POTENCIALCLI] ADD  DEFAULT ((0)) FOR [CFLAGPRIN] ALTER TABLE [" + base_datos + "].[dbo].[POTENCIALCLI] ADD  DEFAULT ('') FOR [CCODTRANS] ALTER TABLE [" + base_datos + "].[dbo].[POTENCIALCLI] ADD  DEFAULT ((1)) FOR [TCL_CODIGO] ALTER TABLE [" + base_datos + "].[dbo].[POTENCIALCLI] ADD  DEFAULT ('') FOR [CONTACTO_COBRANZA] ALTER TABLE [" + base_datos + "].[dbo].[POTENCIALCLI] ADD  DEFAULT ('') FOR [CCODCLAS] ALTER TABLE [" + base_datos + "].[dbo].[POTENCIALCLI]  WITH CHECK ADD  CONSTRAINT [FK_POTENCIALCLI_T_TIPO_CLIENTE] FOREIGN KEY([TCL_CODIGO]) REFERENCES [" + base_datos + "].[dbo].[T_TIPO_CLIENTE] ([TCL_CODIGO]) ALTER TABLE [" + base_datos + "].[dbo].[POTENCIALCLI] CHECK CONSTRAINT [FK_POTENCIALCLI_T_TIPO_CLIENTE] END");
                    BD.Database.ExecuteSqlCommand("IF   NOT EXISTS (SELECT * FROM [" + base_datos + "].[dbo].sysobjects WHERE NAME='CONTACTO_POTENCIAL'  and TYPE='U') BEGIN CREATE TABLE [" + base_datos + "].[dbo].[CONTACTO_POTENCIAL](	[COD_CLIENTE] [char](22) NOT NULL,	[COD_CONTACTO] [numeric](9, 0) NOT NULL,	[CONTACTO] [varchar](100) NOT NULL,	[TELEFONO] [varchar](100) NULL,	[CORREO] [varchar](100) NULL,	[AREA] [varchar](100) NULL,	[CARGO] [varchar](100) NULL,	[CELULAR] [varchar](100) NULL, CONSTRAINT [PK_CONTACTO_POTENCIAL] PRIMARY KEY CLUSTERED (	[COD_CLIENTE] ASC,	[COD_CONTACTO] ASC )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY] ) ON [PRIMARY] END");
                    BD.Database.ExecuteSqlCommand("IF   NOT EXISTS (SELECT * FROM [" + base_datos + "].[dbo].sysobjects WHERE NAME='ESTADO_PROSPECTO'  and TYPE='U') BEGIN CREATE TABLE [" + base_datos + "].[dbo].[ESTADO_PROSPECTO](	[id_estadoOportunidad] [int] NOT NULL,	[decripcion] [varchar](50) NULL,	[color] [varchar](50) NULL,	[posicion] [int] NULL, CONSTRAINT [PK_ESTADO_OPORTUNIDAD] PRIMARY KEY CLUSTERED  (	[id_estadoOportunidad] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY] INSERT [" + base_datos + "].[dbo].[ESTADO_PROSPECTO] ([id_estadoOportunidad], [decripcion], [color], [posicion]) VALUES (1, N'Nuevo', N'#2b76ff', 1)INSERT [" + base_datos + "].[dbo].[ESTADO_PROSPECTO] ([id_estadoOportunidad], [decripcion], [color], [posicion]) VALUES (2, N'En Proceso', N'#ff0000', 2) INSERT [" + base_datos + "].[dbo].[ESTADO_PROSPECTO] ([id_estadoOportunidad], [decripcion], [color], [posicion]) VALUES (3, N'En Negociacion', N'#744700', 3)INSERT [" + base_datos + "].[dbo].[ESTADO_PROSPECTO] ([id_estadoOportunidad], [decripcion], [color], [posicion]) VALUES (4, N'Cerrado', N'#45cc00', 4) END ");
                    BD.Database.ExecuteSqlCommand("IF   NOT EXISTS (SELECT * FROM [" + base_datos + "].[dbo].sysobjects WHERE NAME='ESTATUS_PROSPECTO'  and TYPE='U') BEGIN CREATE TABLE [" + base_datos + "].[dbo].[ESTATUS_PROSPECTO](	[id_estatusProspecto] [int] IDENTITY(1,1) NOT NULL,	[descripcion] [varchar](50) NOT NULL,	[color] [varchar](10) NOT NULL,PRIMARY KEY CLUSTERED (	[id_estatusProspecto] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY] SET IDENTITY_INSERT [" + base_datos + "].[dbo].[ESTATUS_PROSPECTO] ON INSERT [" + base_datos + "].[dbo].[ESTATUS_PROSPECTO] ([id_estatusProspecto], [descripcion], [color]) VALUES (1, N'Flujo', N'#fff') INSERT [" + base_datos + "].[dbo].[ESTATUS_PROSPECTO] ([id_estatusProspecto], [descripcion], [color]) VALUES (2, N'Archivado', N'#3c8dbc') INSERT [" + base_datos + "].[dbo].[ESTATUS_PROSPECTO] ([id_estatusProspecto], [descripcion], [color]) VALUES (3, N'Ganado', N'#28a745') INSERT [" + base_datos + "].[dbo].[ESTATUS_PROSPECTO] ([id_estatusProspecto], [descripcion], [color]) VALUES (4, N'Perdido', N'#dc3545') SET IDENTITY_INSERT [" + base_datos + "].[dbo].[ESTATUS_PROSPECTO] OFF END");
                    BD.Database.ExecuteSqlCommand("IF   NOT EXISTS (SELECT * FROM [" + base_datos + "].[dbo].sysobjects WHERE NAME='RESPONSABLES'  and TYPE='U') BEGIN CREATE TABLE [" + base_datos + "].[dbo].[RESPONSABLES]([id_responsable] [int] NOT NULL,	[id_actividad] [int] NULL,	[id_grupos] [int] NULL,	[id_usuario] [int] NULL, CONSTRAINT [PK_RESPONSABLES_TAREA] PRIMARY KEY CLUSTERED  ( [id_responsable] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY] ALTER TABLE [" + base_datos + "].[dbo].[RESPONSABLES]  WITH CHECK ADD  CONSTRAINT [FK_RESPONSABLES_TAREA_GRUPOS] FOREIGN KEY([id_grupos])REFERENCES [" + base_datos + "].[dbo].[GRUPOS] ([id_grupo]) ALTER TABLE [" + base_datos + "].[dbo].[RESPONSABLES] CHECK CONSTRAINT [FK_RESPONSABLES_TAREA_GRUPOS] END");
                    BD.Database.ExecuteSqlCommand("IF   NOT EXISTS (SELECT * FROM [" + base_datos + "].[dbo].sysobjects WHERE NAME='PRIORIDAD'  and TYPE='U') BEGIN CREATE TABLE [" + base_datos + "].[dbo].[PRIORIDAD](	[id_prioridad] [int] NOT NULL,	[descripcion] [varchar](10) NULL, CONSTRAINT [PK_PRIORIDAD] PRIMARY KEY CLUSTERED  (	[id_prioridad] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY] INSERT [" + base_datos + "].[dbo].[PRIORIDAD] ([id_prioridad], [descripcion]) VALUES (0, N'Sin') INSERT [" + base_datos + "].[dbo].[PRIORIDAD] ([id_prioridad], [descripcion]) VALUES (1, N'Baja')INSERT [" + base_datos + "].[dbo].[PRIORIDAD] ([id_prioridad], [descripcion]) VALUES (2, N'Media') INSERT [" + base_datos + "].[dbo].[PRIORIDAD] ([id_prioridad], [descripcion]) VALUES (3, N'Alta') END");
                    BD.Database.ExecuteSqlCommand("IF   NOT EXISTS (SELECT * FROM [" + base_datos + "].[dbo].sysobjects WHERE NAME='PROSPECTO'  and TYPE='U') BEGIN CREATE TABLE [" + base_datos + "].[dbo].[PROSPECTO](	[id_oportunidad] [varchar](5) NOT NULL,	[nombre] [varchar](50) NULL,	[ingreso] [decimal](10, 2) NULL,	[id_estadoOporunidad] [int] NULL,	[idCliente] [varchar](11) NULL,	[id_prioridad] [int] NULL,	[color] [varchar](10) NULL,	[id_usuario] [varchar](8) NULL,	[cierre_previsto] [varchar](20) NULL,	[id_categoria] [int] NULL,	[notas] [varchar](200) NULL, [id_planificacion] [int] NULL,	[tipoCliente] [int] NULL,	[codigo_tipoMon] [varchar](2) NULL,	[porcentajeGanar] [int] NULL,	[id_estatusProspecto] [int] NOT NULL, CONSTRAINT [PK_OPORTUNIDAD] PRIMARY KEY CLUSTERED (	[id_oportunidad] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY] ALTER TABLE [" + base_datos + "].[dbo].[PROSPECTO] ADD  DEFAULT ((1)) FOR [id_estatusProspecto] ALTER TABLE [" + base_datos + "].[dbo].[PROSPECTO]  WITH CHECK ADD  CONSTRAINT [FK_OPORTUNIDAD_ESTADO_OPORTUNIDAD] FOREIGN KEY([id_estadoOporunidad]) REFERENCES [" + base_datos + "].[dbo].[ESTADO_PROSPECTO] ([id_estadoOportunidad])  ALTER TABLE [" + base_datos + "].[dbo].[PROSPECTO] CHECK CONSTRAINT [FK_OPORTUNIDAD_ESTADO_OPORTUNIDAD] ALTER TABLE [" + base_datos + "].[dbo].[PROSPECTO]  WITH CHECK ADD  CONSTRAINT [FK_OPORTUNIDAD_PRIORIDAD] FOREIGN KEY([id_prioridad]) REFERENCES [" + base_datos + "].[dbo].[PRIORIDAD] ([id_prioridad]) ALTER TABLE [" + base_datos + "].[dbo].[PROSPECTO] CHECK CONSTRAINT [FK_OPORTUNIDAD_PRIORIDAD] ALTER TABLE [" + base_datos + "].[dbo].[PROSPECTO]  WITH NOCHECK ADD  CONSTRAINT [FK_PROSPECTO_ESTATUS_PROSPECTO] FOREIGN KEY([id_estatusProspecto]) REFERENCES [" + base_datos + "].[dbo].[ESTATUS_PROSPECTO] ([id_estatusProspecto]) ALTER TABLE [" + base_datos + "].[dbo].[PROSPECTO] NOCHECK CONSTRAINT [FK_PROSPECTO_ESTATUS_PROSPECTO] ALTER TABLE [" + base_datos + "].[dbo].[PROSPECTO]  WITH CHECK ADD  CONSTRAINT [FK_PROSPECTO_TIPO_MONEDA] FOREIGN KEY([codigo_tipoMon]) REFERENCES [" + base_datos + "].[dbo].[TIPO_MONEDA] ([TIPOMON_CODIGO]) ALTER TABLE [" + base_datos + "].[dbo].[PROSPECTO] CHECK CONSTRAINT [FK_PROSPECTO_TIPO_MONEDA] ALTER TABLE [" + base_datos + "].[dbo].[PROSPECTO] ADD	[fecha_inicial] [datetime] NULL; ALTER TABLE [" + base_datos + "].[dbo].[PROSPECTO] ADD	[fecha_final] [datetime] NULL; ALTER TABLE [" + base_datos + "].[dbo].[PROSPECTO] ADD [monto_logrado] [decimal] NULL; END");
                    BD.Database.ExecuteSqlCommand("IF   NOT EXISTS (SELECT * FROM [" + base_datos + "].[dbo].sysobjects WHERE NAME='CRONOGRAMA'  and TYPE='U') BEGIN  CREATE TABLE [" + base_datos + "].[dbo].[CRONOGRAMA](	[id_cronograma] [int] IDENTITY(1,1) NOT NULL,	[observacion] [nvarchar](1000) NULL,	[id_tipoActividad] [int] NOT NULL,	[descripcion] [nvarchar](1000) NULL,	[id_oportunidad] [varchar](5) NOT NULL,	[fecha_inicial] [datetime] NOT NULL,	[fecha_final] [datetime] NOT NULL,	[estatus] [int] NOT NULL,	[USU_CODIGO] [nvarchar](20) NULL, [respuesta] [nvarchar](1000) NULL, PRIMARY KEY CLUSTERED (	[id_cronograma] ASC )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY] ) ON [PRIMARY] ALTER TABLE [" + base_datos + "].[dbo].[CRONOGRAMA] ADD  DEFAULT (getdate()) FOR [fecha_inicial] ALTER TABLE [" + base_datos + "].[dbo].[CRONOGRAMA] ADD  DEFAULT (getdate()) FOR [fecha_final] ALTER TABLE [" + base_datos + "].[dbo].[CRONOGRAMA] ADD  DEFAULT ((0)) FOR [estatus] ALTER TABLE [" + base_datos + "].[dbo].[CRONOGRAMA]  WITH NOCHECK ADD  CONSTRAINT [fk_cronograma_prospecto] FOREIGN KEY([id_oportunidad]) REFERENCES [" + base_datos + "].[dbo].[PROSPECTO] ([id_oportunidad]) ALTER TABLE [" + base_datos + "].[dbo].[CRONOGRAMA] CHECK CONSTRAINT [fk_cronograma_prospecto] ALTER TABLE [" + base_datos + "].[dbo].[CRONOGRAMA]  WITH CHECK ADD  CONSTRAINT [fk_cronograma_tipo_actividad] FOREIGN KEY([id_tipoActividad]) REFERENCES [" + base_datos + "].[dbo].[TIPO_ACTIVIDAD] ([id_tipoActividad]) ALTER TABLE [" + base_datos + "].[dbo].[CRONOGRAMA] CHECK CONSTRAINT [fk_cronograma_tipo_actividad] END ");
                    BD.Database.ExecuteSqlCommand("IF   NOT EXISTS (SELECT * FROM [" + base_datos + "].[dbo].sysobjects WHERE NAME='HISTORICO'  and TYPE='U') BEGIN CREATE TABLE [" + base_datos + "].[dbo].[HISTORICO](	[id_historico] [int] IDENTITY(1,1) NOT NULL,	[descripcion] [nvarchar](1000) NULL,	[titulo] [nvarchar](100) NULL,	[fecha] [datetime] NOT NULL,	[id-estatusOp] [int] NULL,	[id_usuario] [nvarchar](20)NOT NULL,	[id_EstadoProspecto] [int] NULL,	[id_oportunidad] [varchar](5) NULL,PRIMARY KEY CLUSTERED (	[id_historico] ASC )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY] ) ON [PRIMARY] ALTER TABLE [" + base_datos + "].[dbo].[HISTORICO]  WITH CHECK ADD  CONSTRAINT [FK_HISTORICO_OPORTUNIDAD] FOREIGN KEY([id_oportunidad]) REFERENCES [" + base_datos + "].[dbo].[PROSPECTO] ([id_oportunidad]) ALTER TABLE [" + base_datos + "].[dbo].[HISTORICO] CHECK CONSTRAINT [FK_HISTORICO_OPORTUNIDAD] ALTER TABLE [" + base_datos + "].[dbo].[HISTORICO] ADD  DEFAULT (getdate()) FOR [fecha] END");
                    empresas.Add(i);

                //}
            }
            if (empresas.Count() != 0)
            {
                Session["EMPRESAS"] = empresas;
            }
            else
            {

                Session["EMPRESAS"] = null;
            }
        }


        public void Verificar_Data_User_Admin(CRM_Usuarios_Rol model)
        {
            BDWENCOEntities BD = new BDWENCOEntities(CD.ConexDinamica(Session["datasour"].ToString(), Session["catalog"].ToString(), Session["user"].ToString(), Session["password"].ToString()));

            var empresas = new List<SP_EMPRESA_USUARIO_CRM_Result>();
            var empresas_asociadas = new List<CRM_Usuarios_Rol>();
            empresas_asociadas = BD.CRM_Usuarios_Rol.Where(b => b.alias == model.alias).ToList();

            foreach (var i in empresas_asociadas)
            {
                string base_datos = "CRM";
                /*SP_PORTAL_VERIFICAR_DATABASE_Result validacion2 = BD.SP_PORTAL_VERIFICAR_DATABASE1(base_datos).First();
                if (validacion2.data != "0")
                {*/
                    BD.Database.ExecuteSqlCommand("IF   NOT EXISTS (SELECT * FROM [" + base_datos + "].[dbo].sysobjects WHERE NAME='TIPO_MONEDA'  and TYPE='U')  BEGIN CREATE TABLE[" + base_datos + "].[dbo].[TIPO_MONEDA]([TIPOMON_CODIGO] [varchar](2) PRIMARY KEY,[TIPOMON_DESCRIPCION][varchar](20) NULL,[TIPOMON_NACIONALIDAD][bit] NOT NULL default 0,[TIPOMON_SIMBOLO][varchar](2) NULL); INSERT INTO TIPO_MONEDA(TIPOMON_CODIGO,TIPOMON_DESCRIPCION,TIPOMON_NACIONALIDAD,TIPOMON_SIMBOLO) VALUES('ME','DOLARES',0,'$'); INSERT INTO TIPO_MONEDA(TIPOMON_CODIGO,TIPOMON_DESCRIPCION,TIPOMON_NACIONALIDAD,TIPOMON_SIMBOLO) VALUES('MN','BOLIVARES',1,'Bs'); END ");
                    BD.Database.ExecuteSqlCommand("IF   NOT EXISTS (SELECT * FROM [" + base_datos + "].[dbo].sysobjects WHERE NAME='GRUPOS'  and TYPE='U')  BEGIN CREATE TABLE[" + base_datos + "].[dbo].[GRUPOS]( [id_grupo][int] NOT NULL, [descripcion][varchar](50) NULL,CONSTRAINT[PK_GRUPOS] PRIMARY KEY CLUSTERED([id_grupo] ASC )WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]) ON[PRIMARY] END ");
                    BD.Database.ExecuteSqlCommand("IF   NOT EXISTS(SELECT * FROM[" + base_datos + "].[dbo].sysobjects WHERE NAME = 'GRUPO_USUARIO'  and TYPE = 'U') BEGIN CREATE TABLE[" + base_datos + "].[dbo].[GRUPO_USUARIO](    [id_grupoUuario][int] NOT NULL,    [id_grupo][int] NULL,    [int_usuario][int] NULL, CONSTRAINT[PK_GRUPO_USUARIO] PRIMARY KEY CLUSTERED (   [id_grupoUuario] ASC )WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY] ) ON[PRIMARY] ALTER TABLE[" + base_datos + "].[dbo].[GRUPO_USUARIO]  WITH CHECK ADD CONSTRAINT[FK_GRUPO_USUARIO_GRUPOS] FOREIGN KEY([id_grupo]) REFERENCES[" + base_datos + "].[dbo].[GRUPOS] ([id_grupo]) ALTER TABLE[" + base_datos + "].[dbo].[GRUPO_USUARIO] CHECK CONSTRAINT[FK_GRUPO_USUARIO_GRUPOS] END     ");
                    BD.Database.ExecuteSqlCommand("IF   NOT EXISTS(SELECT * FROM[" + base_datos + "].[dbo].sysobjects WHERE NAME = 'TIPO_ACTIVIDAD'  and TYPE = 'U') BEGIN CREATE TABLE [" + base_datos + "].[dbo].[TIPO_ACTIVIDAD]( [id_tipoActividad][int] IDENTITY(1, 1) NOT NULL, [descripcion][nvarchar](100) NULL, [estado][bit] NOT NULL, [color][nvarchar](20) NOT NULL, PRIMARY KEY CLUSTERED ( [id_tipoActividad] ASC )WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY] ) ON[PRIMARY] SET IDENTITY_INSERT[" + base_datos + "].[dbo].[TIPO_ACTIVIDAD] ON INSERT[" + base_datos + "].[dbo].[TIPO_ACTIVIDAD] ([id_tipoActividad], [descripcion], [estado], [color]) VALUES(1, N'Llamada', 1, N'#0800ff') INSERT[" + base_datos + "].[dbo].[TIPO_ACTIVIDAD] ([id_tipoActividad], [descripcion], [estado], [color]) VALUES(2, N'Correo Electrónico', 1, N'#00ff1d') INSERT[" + base_datos + "].[dbo].[TIPO_ACTIVIDAD] ([id_tipoActividad], [descripcion], [estado], [color]) VALUES(3, N'Visita Personal', 1, N'#ffbb00') INSERT[" + base_datos + "].[dbo].[TIPO_ACTIVIDAD] ([id_tipoActividad], [descripcion], [estado], [color]) VALUES(4, N'Reunión Digital', 1, N'#a100ff') SET IDENTITY_INSERT[" + base_datos + "].[dbo].[TIPO_ACTIVIDAD] OFF SET ANSI_PADDING ON ALTER TABLE[" + base_datos + "].[dbo].[TIPO_ACTIVIDAD] ADD CONSTRAINT[uq_ta_descripcion] UNIQUE NONCLUSTERED ( [descripcion] ASC )WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY] ALTER TABLE[" + base_datos + "].[dbo].[TIPO_ACTIVIDAD]  ADD DEFAULT((1)) FOR[estado] ALTER TABLE[" + base_datos + "].[dbo].[TIPO_ACTIVIDAD]ADD DEFAULT('#FFF') FOR[color] END ");
                    BD.Database.ExecuteSqlCommand("IF   NOT EXISTS (SELECT * FROM [" + base_datos + "].[dbo].sysobjects WHERE NAME='POTENCIALCLI'  and TYPE='U') BEGIN CREATE TABLE [" + base_datos + "].[dbo].[POTENCIALCLI](	[CCODCLI] [varchar](11) NOT NULL,	[CNOMCLI] [varchar](100) NULL,	[CDIRCLI] [varchar](100) NULL,	[CTELEFO] [varchar](30) NULL,	[CNUMRUC] [varchar](11) NULL,	[CDOCIDEN] [varchar](15) NULL,	[CFLADES] [varchar](1) NULL,	[NPORDES] [numeric](15, 6) NULL,	[CTIPVTA] [varchar](4) NULL,	[CESTADO] [varchar](1) NULL,	[DFECINS] [smalldatetime] NULL,	[CNOMREP] [varchar](30) NULL,	[CDISTRI] [varchar](8) NULL,	[CUSUARI] [varchar](8) NULL,	[DFECCRE] [smalldatetime] NULL,	[DFECMOD] [smalldatetime] NULL,	[CTIPPRE] [varchar](4) NULL,	[CVENDE] [varchar](2) NULL,	[CZONVTA] [varchar](8) NULL,	[CPAIS] [varchar](30) NULL,	[CDEPT] [varchar](30) NULL,	[CPROV] [varchar](30) NULL,	[DIRENT] [varchar](100) NULL,	[LOCENT] [varchar](3) NULL,	[LOCCLI] [varchar](3) NULL,	[LOCEST] [varchar](2) NULL,	[ZONVTA] [varchar](5) NULL,	[DIAATE] [varchar](1) NULL,	[SITCLI] [varchar](2) NULL,	[TIPIDE] [varchar](2) NULL,	[REGVTA] [varchar](7) NULL,	[MONCRE] [varchar](2) NULL,	[LMCRUS] [numeric](15, 6) NULL,	[LMCRMN] [numeric](15, 6) NULL,	[SALDMN] [numeric](18, 6) NULL,	[SALDUS] [numeric](18, 6) NULL,	[LIMFAC] [numeric](15, 6) NULL,	[TOTFAC] [numeric](15, 6) NULL,	[TOTLEP] [numeric](15, 6) NULL,	[TOTCHD] [numeric](15, 6) NULL,	[FULABO] [varchar](6) NULL,	[FECUFA] [varchar](6) NULL,	[NROSOL] [varchar](6) NULL,	[OBSERV] [varchar](30) NULL,	[DOCVEN] [numeric](15, 6) NULL,	[DIAVEN] [numeric](15, 6) NULL,	[NROORD] [varchar](10) NULL,	[MORLET] [numeric](15, 6) NULL,	[MORFAC] [numeric](15, 6) NULL,	[CHEDEV] [numeric](15, 6) NULL,	[LETPRO] [numeric](15, 6) NULL,	[CTIPCLI] [varchar](2) NULL,	[CGIRNEG] [varchar](2) NULL,	[CTERRIT] [varchar](2) NULL,	[CRUTA] [varchar](2) NULL,	[CSEGMEN] [varchar](3) NULL,	[CUBISEG] [varchar](3) NULL,	[CCODBAN] [varchar](3) NULL,	[CNUMCTA] [varchar](16) NULL,	[CFREVIS] [varchar](8) NULL,	[CHORATE] [varchar](20) NULL,	[CTIPATE] [varchar](2) NULL,	[CNUMFAX] [varchar](15) NULL,	[CEMAIL] [varchar](200) NULL,	[CHOST] [varchar](80) NULL,	[CZONPOS] [varchar](5) NULL,	[COMENTA] [varchar](500) NULL,	[RETEN] [bit] NULL,	[CFLAGPRIN] [bit] NOT NULL,	[CCODTRANS] [varchar](11) NOT NULL,	[SIN_CONTROL_LIMCREDITO] [bit] NULL,	[SUBCLA01] [varchar](30) NULL,	[SUBCLA02] [varchar](30) NULL,	[ZON_CODIGO] [varchar](20) NULL,	[CTIPO_DOCUMENTO] [varchar](2) NULL,	[CAPELLIDO_PATERNO] [varchar](20) NULL,	[CAPELLIDO_MATERNO] [varchar](20) NULL,	[CPRIMER_NOMBRE] [varchar](20) NULL,	[CSEGUNDO_NOMBRE] [varchar](20) NULL,	[TCL_CODIGO] [varchar](3) NOT NULL,	[NRO_AUTORIZACION_DIGEMID] [varchar](50) NULL,	[CONTACTO_COBRANZA] [varchar](40) NOT NULL,	[CCODCLAS] [varchar](8) NOT NULL,	[FLGPORTAL_CLIENTES] [bit] NULL,	[ULTIMO_TC_VTA] [numeric](18, 6) NULL,	[DESCUENTO_ESP] [numeric](18, 6) NULL,	[CEMAIL_CONTACTO] [varchar](120) NULL,	[UBIGEO] [varchar](12) NULL,	[FEC_INACTIVO_BLOQUEADO] [datetime] NULL,	[COD_AUDITORIA] [varchar](12) NULL, CONSTRAINT [PK_POTENCIALCLI] PRIMARY KEY CLUSTERED ([CCODCLI] ASC )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY] ALTER TABLE [" + base_datos + "].[dbo].[POTENCIALCLI] ADD  DEFAULT ((0)) FOR [NPORDES] ALTER TABLE [" + base_datos + "].[dbo].[POTENCIALCLI] ADD  DEFAULT ((0)) FOR [LMCRUS] ALTER TABLE [" + base_datos + "].[dbo].[POTENCIALCLI] ADD  DEFAULT ((0)) FOR [LMCRMN] ALTER TABLE [" + base_datos + "].[dbo].[POTENCIALCLI] ADD  DEFAULT ((0)) FOR [SALDMN] ALTER TABLE [" + base_datos + "].[dbo].[POTENCIALCLI] ADD  DEFAULT ((0)) FOR [SALDUS] ALTER TABLE [" + base_datos + "].[dbo].[POTENCIALCLI] ADD  DEFAULT ((0)) FOR [LIMFAC] ALTER TABLE [" + base_datos + "].[dbo].[POTENCIALCLI] ADD  DEFAULT ((0)) FOR [TOTFAC] ALTER TABLE [" + base_datos + "].[dbo].[POTENCIALCLI] ADD  DEFAULT ((0)) FOR [TOTLEP] ALTER TABLE [" + base_datos + "].[dbo].[POTENCIALCLI] ADD  DEFAULT ((0)) FOR [TOTCHD] ALTER TABLE [" + base_datos + "].[dbo].[POTENCIALCLI] ADD  DEFAULT ((0)) FOR [DOCVEN] ALTER TABLE [" + base_datos + "].[dbo].[POTENCIALCLI] ADD  DEFAULT ((0)) FOR [DIAVEN] ALTER TABLE [" + base_datos + "].[dbo].[POTENCIALCLI] ADD  DEFAULT ((0)) FOR [MORLET] ALTER TABLE [" + base_datos + "].[dbo].[POTENCIALCLI] ADD  DEFAULT ((0)) FOR [MORFAC] ALTER TABLE [" + base_datos + "].[dbo].[POTENCIALCLI] ADD  DEFAULT ((0)) FOR [CHEDEV] ALTER TABLE [" + base_datos + "].[dbo].[POTENCIALCLI] ADD  DEFAULT ((0)) FOR [LETPRO] ALTER TABLE [" + base_datos + "].[dbo].[POTENCIALCLI] ADD  DEFAULT ((0)) FOR [CFLAGPRIN] ALTER TABLE [" + base_datos + "].[dbo].[POTENCIALCLI] ADD  DEFAULT ('') FOR [CCODTRANS] ALTER TABLE [" + base_datos + "].[dbo].[POTENCIALCLI] ADD  DEFAULT ((1)) FOR [TCL_CODIGO] ALTER TABLE [" + base_datos + "].[dbo].[POTENCIALCLI] ADD  DEFAULT ('') FOR [CONTACTO_COBRANZA] ALTER TABLE [" + base_datos + "].[dbo].[POTENCIALCLI] ADD  DEFAULT ('') FOR [CCODCLAS] END");
                    BD.Database.ExecuteSqlCommand("IF   NOT EXISTS (SELECT * FROM [" + base_datos + "].[dbo].sysobjects WHERE NAME='CONTACTO_POTENCIAL'  and TYPE='U') BEGIN CREATE TABLE [" + base_datos + "].[dbo].[CONTACTO_POTENCIAL](	[COD_CLIENTE] [char](22) NOT NULL,	[COD_CONTACTO] [numeric](9, 0) NOT NULL,	[CONTACTO] [varchar](100) NOT NULL,	[TELEFONO] [varchar](100) NULL,	[CORREO] [varchar](100) NULL,	[AREA] [varchar](100) NULL,	[CARGO] [varchar](100) NULL,	[CELULAR] [varchar](100) NULL, CONSTRAINT [PK_CONTACTO_POTENCIAL] PRIMARY KEY CLUSTERED (	[COD_CLIENTE] ASC,	[COD_CONTACTO] ASC )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY] ) ON [PRIMARY] END");
                    BD.Database.ExecuteSqlCommand("IF   NOT EXISTS (SELECT * FROM [" + base_datos + "].[dbo].sysobjects WHERE NAME='ESTADO_PROSPECTO'  and TYPE='U') BEGIN CREATE TABLE [" + base_datos + "].[dbo].[ESTADO_PROSPECTO](	[id_estadoOportunidad] [int] NOT NULL,	[decripcion] [varchar](50) NULL,	[color] [varchar](50) NULL,	[posicion] [int] NULL, CONSTRAINT [PK_ESTADO_OPORTUNIDAD] PRIMARY KEY CLUSTERED  (	[id_estadoOportunidad] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY] INSERT [" + base_datos + "].[dbo].[ESTADO_PROSPECTO] ([id_estadoOportunidad], [decripcion], [color], [posicion]) VALUES (1, N'Nuevo', N'#2b76ff', 1)INSERT [" + base_datos + "].[dbo].[ESTADO_PROSPECTO] ([id_estadoOportunidad], [decripcion], [color], [posicion]) VALUES (2, N'En Proceso', N'#ff0000', 2) INSERT [" + base_datos + "].[dbo].[ESTADO_PROSPECTO] ([id_estadoOportunidad], [decripcion], [color], [posicion]) VALUES (3, N'En Negociacion', N'#744700', 3)INSERT [" + base_datos + "].[dbo].[ESTADO_PROSPECTO] ([id_estadoOportunidad], [decripcion], [color], [posicion]) VALUES (4, N'Cerrado', N'#45cc00', 4) END ");
                    BD.Database.ExecuteSqlCommand("IF   NOT EXISTS (SELECT * FROM [" + base_datos + "].[dbo].sysobjects WHERE NAME='ESTATUS_PROSPECTO'  and TYPE='U') BEGIN CREATE TABLE [" + base_datos + "].[dbo].[ESTATUS_PROSPECTO](	[id_estatusProspecto] [int] IDENTITY(1,1) NOT NULL,	[descripcion] [varchar](50) NOT NULL,	[color] [varchar](10) NOT NULL,PRIMARY KEY CLUSTERED (	[id_estatusProspecto] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY] SET IDENTITY_INSERT [" + base_datos + "].[dbo].[ESTATUS_PROSPECTO] ON INSERT [" + base_datos + "].[dbo].[ESTATUS_PROSPECTO] ([id_estatusProspecto], [descripcion], [color]) VALUES (1, N'Flujo', N'#fff') INSERT [" + base_datos + "].[dbo].[ESTATUS_PROSPECTO] ([id_estatusProspecto], [descripcion], [color]) VALUES (2, N'Archivado', N'#3c8dbc') INSERT [" + base_datos + "].[dbo].[ESTATUS_PROSPECTO] ([id_estatusProspecto], [descripcion], [color]) VALUES (3, N'Ganado', N'#28a745') INSERT [" + base_datos + "].[dbo].[ESTATUS_PROSPECTO] ([id_estatusProspecto], [descripcion], [color]) VALUES (4, N'Perdido', N'#dc3545') SET IDENTITY_INSERT [" + base_datos + "].[dbo].[ESTATUS_PROSPECTO] OFF END");
                    BD.Database.ExecuteSqlCommand("IF   NOT EXISTS (SELECT * FROM [" + base_datos + "].[dbo].sysobjects WHERE NAME='RESPONSABLES'  and TYPE='U') BEGIN CREATE TABLE [" + base_datos + "].[dbo].[RESPONSABLES]([id_responsable] [int] NOT NULL,	[id_actividad] [int] NULL,	[id_grupos] [int] NULL,	[id_usuario] [int] NULL, CONSTRAINT [PK_RESPONSABLES_TAREA] PRIMARY KEY CLUSTERED  ( [id_responsable] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY] ALTER TABLE [" + base_datos + "].[dbo].[RESPONSABLES]  WITH CHECK ADD  CONSTRAINT [FK_RESPONSABLES_TAREA_GRUPOS] FOREIGN KEY([id_grupos])REFERENCES [" + base_datos + "].[dbo].[GRUPOS] ([id_grupo]) ALTER TABLE [" + base_datos + "].[dbo].[RESPONSABLES] CHECK CONSTRAINT [FK_RESPONSABLES_TAREA_GRUPOS] END");
                    BD.Database.ExecuteSqlCommand("IF   NOT EXISTS (SELECT * FROM [" + base_datos + "].[dbo].sysobjects WHERE NAME='PRIORIDAD'  and TYPE='U') BEGIN CREATE TABLE [" + base_datos + "].[dbo].[PRIORIDAD](	[id_prioridad] [int] NOT NULL,	[descripcion] [varchar](10) NULL, CONSTRAINT [PK_PRIORIDAD] PRIMARY KEY CLUSTERED  (	[id_prioridad] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY] INSERT [" + base_datos + "].[dbo].[PRIORIDAD] ([id_prioridad], [descripcion]) VALUES (0, N'Sin') INSERT [" + base_datos + "].[dbo].[PRIORIDAD] ([id_prioridad], [descripcion]) VALUES (1, N'Baja')INSERT [" + base_datos + "].[dbo].[PRIORIDAD] ([id_prioridad], [descripcion]) VALUES (2, N'Media') INSERT [" + base_datos + "].[dbo].[PRIORIDAD] ([id_prioridad], [descripcion]) VALUES (3, N'Alta') END");
                    BD.Database.ExecuteSqlCommand("IF   NOT EXISTS (SELECT * FROM [" + base_datos + "].[dbo].sysobjects WHERE NAME='PROSPECTO'  and TYPE='U') BEGIN CREATE TABLE [" + base_datos + "].[dbo].[PROSPECTO](	[id_oportunidad] [varchar](5) NOT NULL,	[nombre] [varchar](50) NULL,	[ingreso] [decimal](10, 2) NULL,	[id_estadoOporunidad] [int] NULL,	[idCliente] [varchar](11) NULL,	[id_prioridad] [int] NULL,	[color] [varchar](10) NULL,	[id_usuario] [varchar](8) NULL,	[cierre_previsto] [varchar](20) NULL,	[id_categoria] [int] NULL,	[notas] [varchar](200) NULL, [id_planificacion] [int] NULL,	[tipoCliente] [int] NULL,	[codigo_tipoMon] [varchar](2) NULL,	[porcentajeGanar] [int] NULL,	[id_estatusProspecto] [int] NOT NULL, CONSTRAINT [PK_OPORTUNIDAD] PRIMARY KEY CLUSTERED (	[id_oportunidad] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY] ALTER TABLE [" + base_datos + "].[dbo].[PROSPECTO] ADD  DEFAULT ((1)) FOR [id_estatusProspecto] ALTER TABLE [" + base_datos + "].[dbo].[PROSPECTO]  WITH CHECK ADD  CONSTRAINT [FK_OPORTUNIDAD_ESTADO_OPORTUNIDAD] FOREIGN KEY([id_estadoOporunidad]) REFERENCES [" + base_datos + "].[dbo].[ESTADO_PROSPECTO] ([id_estadoOportunidad])  ALTER TABLE [" + base_datos + "].[dbo].[PROSPECTO] CHECK CONSTRAINT [FK_OPORTUNIDAD_ESTADO_OPORTUNIDAD] ALTER TABLE [" + base_datos + "].[dbo].[PROSPECTO]  WITH CHECK ADD  CONSTRAINT [FK_OPORTUNIDAD_PRIORIDAD] FOREIGN KEY([id_prioridad]) REFERENCES [" + base_datos + "].[dbo].[PRIORIDAD] ([id_prioridad]) ALTER TABLE [" + base_datos + "].[dbo].[PROSPECTO] CHECK CONSTRAINT [FK_OPORTUNIDAD_PRIORIDAD] ALTER TABLE [" + base_datos + "].[dbo].[PROSPECTO]  WITH NOCHECK ADD  CONSTRAINT [FK_PROSPECTO_ESTATUS_PROSPECTO] FOREIGN KEY([id_estatusProspecto]) REFERENCES [" + base_datos + "].[dbo].[ESTATUS_PROSPECTO] ([id_estatusProspecto]) ALTER TABLE [" + base_datos + "].[dbo].[PROSPECTO] NOCHECK CONSTRAINT [FK_PROSPECTO_ESTATUS_PROSPECTO] ALTER TABLE [" + base_datos + "].[dbo].[PROSPECTO]  WITH CHECK ADD  CONSTRAINT [FK_PROSPECTO_TIPO_MONEDA] FOREIGN KEY([codigo_tipoMon]) REFERENCES [" + base_datos + "].[dbo].[TIPO_MONEDA] ([TIPOMON_CODIGO]) ALTER TABLE [" + base_datos + "].[dbo].[PROSPECTO] CHECK CONSTRAINT [FK_PROSPECTO_TIPO_MONEDA] ALTER TABLE [" + base_datos + "].[dbo].[PROSPECTO] ADD	[fecha_inicial] [datetime] NULL; ALTER TABLE [" + base_datos + "].[dbo].[PROSPECTO] ADD	[fecha_final] [datetime] NULL; ALTER TABLE [" + base_datos + "].[dbo].[PROSPECTO] ADD [monto_logrado] [decimal] NULL; END");
                    BD.Database.ExecuteSqlCommand("IF   NOT EXISTS (SELECT * FROM [" + base_datos + "].[dbo].sysobjects WHERE NAME='CRONOGRAMA'  and TYPE='U') BEGIN  CREATE TABLE [" + base_datos + "].[dbo].[CRONOGRAMA](	[id_cronograma] [int] IDENTITY(1,1) NOT NULL,	[observacion] [nvarchar](1000) NULL,	[id_tipoActividad] [int] NOT NULL,	[descripcion] [nvarchar](1000) NULL,	[id_oportunidad] [varchar](5) NOT NULL,	[fecha_inicial] [datetime] NOT NULL,	[fecha_final] [datetime] NOT NULL,	[estatus] [int] NOT NULL,	[USU_CODIGO] [nvarchar](20) NULL, [respuesta] [nvarchar](1000) NULL, PRIMARY KEY CLUSTERED (	[id_cronograma] ASC )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY] ) ON [PRIMARY] ALTER TABLE [" + base_datos + "].[dbo].[CRONOGRAMA] ADD  DEFAULT (getdate()) FOR [fecha_inicial] ALTER TABLE [" + base_datos + "].[dbo].[CRONOGRAMA] ADD  DEFAULT (getdate()) FOR [fecha_final] ALTER TABLE [" + base_datos + "].[dbo].[CRONOGRAMA] ADD  DEFAULT ((0)) FOR [estatus] ALTER TABLE [" + base_datos + "].[dbo].[CRONOGRAMA]  WITH NOCHECK ADD  CONSTRAINT [fk_cronograma_prospecto] FOREIGN KEY([id_oportunidad]) REFERENCES [" + base_datos + "].[dbo].[PROSPECTO] ([id_oportunidad]) ALTER TABLE [" + base_datos + "].[dbo].[CRONOGRAMA] CHECK CONSTRAINT [fk_cronograma_prospecto] ALTER TABLE [" + base_datos + "].[dbo].[CRONOGRAMA]  WITH CHECK ADD  CONSTRAINT [fk_cronograma_tipo_actividad] FOREIGN KEY([id_tipoActividad]) REFERENCES [" + base_datos + "].[dbo].[TIPO_ACTIVIDAD] ([id_tipoActividad]) ALTER TABLE [" + base_datos + "].[dbo].[CRONOGRAMA] CHECK CONSTRAINT [fk_cronograma_tipo_actividad] END ");
                    BD.Database.ExecuteSqlCommand("IF   NOT EXISTS (SELECT * FROM [" + base_datos + "].[dbo].sysobjects WHERE NAME='HISTORICO'  and TYPE='U') BEGIN CREATE TABLE [" + base_datos + "].[dbo].[HISTORICO](	[id_historico] [int] IDENTITY(1,1) NOT NULL,	[descripcion] [nvarchar](1000) NULL,	[titulo] [nvarchar](100) NULL,	[fecha] [datetime] NOT NULL,	[id-estatusOp] [int] NULL,	[id_usuario] [nvarchar](20)NOT NULL,	[id_EstadoProspecto] [int] NULL,	[id_oportunidad] [varchar](5) NULL,PRIMARY KEY CLUSTERED (	[id_historico] ASC )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY] ) ON [PRIMARY] ALTER TABLE [" + base_datos + "].[dbo].[HISTORICO]  WITH CHECK ADD  CONSTRAINT [FK_HISTORICO_OPORTUNIDAD] FOREIGN KEY([id_oportunidad]) REFERENCES [" + base_datos + "].[dbo].[PROSPECTO] ([id_oportunidad]) ALTER TABLE [" + base_datos + "].[dbo].[HISTORICO] CHECK CONSTRAINT [FK_HISTORICO_OPORTUNIDAD] ALTER TABLE [" + base_datos + "].[dbo].[HISTORICO] ADD  DEFAULT (getdate()) FOR [fecha] END");

                    
                        empresas.Add(BD.SP_EMPRESA_USUARIO_CRM().Where(x => x.EMP_CODIGO == i.codigoEmpresa).First());
                   

                //}
            }

            if (empresas.Count() != 0)
            {
                Session["EMPRESAS"] = empresas;
            }
            else
            {
                Session["EMPRESAS"] = null;
            }
        }




        public ActionResult Actualizar(string codigo)
        {
            BDWENCOEntities BD = new BDWENCOEntities(CD.ConexDinamica(Session["datasour"].ToString(), Session["catalog"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
            SP_EMPRESA_USUARIO_CRM_Result empresa = BD.SP_EMPRESA_USUARIO_CRM().Where(x => x.EMP_CODIGO == codigo).FirstOrDefault();
            Session["codigo"] = empresa.EMP_CODIGO;
            Session["empresa"] = empresa;
            CRM_Usuarios_Rol oUser = (CRM_Usuarios_Rol)Session["user_logueado"];
            Session["catalog_user"] = codigo + "BDCOMUN";
            CRM_Usuarios_Rol usuario_rol = BD.CRM_Usuarios_Rol.Where(x => x.alias == oUser.alias).Where(x => x.codigoEmpresa == empresa.EMP_CODIGO).FirstOrDefault();
            Session["user_logueado"] = usuario_rol;
            Session["alias"] = usuario_rol.alias;
            return RedirectToAction("Index", "Flujo");
        }


        public ActionResult cerrar()
        {
            Session.RemoveAll();

            return RedirectToAction("Login", "Autentificacion");
        }




    }
}
