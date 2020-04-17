using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using siredd_vf.Models;
using System.IO;
using Dapper;
using System.Configuration;
using System.Data.SqlClient;
using Microsoft.AspNet.Identity;
using System.Web.Security;

using siredd_vf.Services;

namespace siredd_vf.Controllers
{
    public class GererINDICATEURController : Controller
    {
        private DBSIREDDEntities db = new DBSIREDDEntities();
        private DBSIREDDEntities1 db1 = new DBSIREDDEntities1();

        // GET: GererINDICATEUR
       



        public ActionResult CarteScolaire()
        {
            return View();
        }

        public ActionResult SigWeb()
        {
            return View();
        }



        public JsonResult GetSiGDataWeb()
        {
            /* get data from model DataSigWeb*/
            // var listDatafromBase = _servicelistBox.GetDataSIGWebEtabs();
            var listDatafromBase = db.DataSIGWebEtab.ToList();


            /* mapping avec le format Json */
            var rootObject = new RootObject();
            rootObject.type = "FeatureCollection";
            rootObject.name = "ecole_geojson";
            rootObject.crs = new Crs();
            rootObject.crs.type = "name";
            rootObject.crs.properties = new CrsProperties() { name = "urn:ogc:def:crs:OGC:1.3:CRS84" };
            rootObject.features = new List<Feature>();
           // var x = -5.094;
           // var y = 34.011;
             foreach (var item in listDatafromBase)
              {
                  Feature feature = new Feature();
                  feature.type = "Feature";
                  feature.properties = new DataEtab();
                  feature.properties.cd_etab = item.CD_ETAB;
                  feature.properties.ll_reg = item.NOM_ETABL;

                feature.geometry = new Geometry();
                  feature.geometry.type = "MultiPoint";
                  feature.geometry.coordinates = new List<List<double>>();
                  feature.geometry.coordinates.Add(new List<double>() { Convert.ToDouble(item.X), Convert.ToDouble(item.Y) });
                  rootObject.features.Add(feature);
              }

            var jsonResult = Json(rootObject, JsonRequestBehavior.AllowGet);  //Json(veryLargeCollection, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
          //  return Json(rootObject, JsonRequestBehavior.AllowGet);
        }


      // [Authorize(Roles = "admin , super-admin, partenaire")]
        public ActionResult Index(int? id, string mot)
        {
            ViewBag.domaines = db.DOMAINE.Where(d => d.ID_REGION == 13).ToList();
            ViewBag.themes = db.THEME.Where(d => d.ID_REGION == 13).ToList();
            ViewBag.soustheme = db.SOUS_THEME.Where(d => d.ID_REGION == 13).ToList();
            ViewBag.organisations = db.ORGANISATION.Where(o => o.ID_REGION == 13).ToList();
            ViewBag.recherche = mot;
            List<DETAIL_INDICATEUR> liste_indicateurs = new List<DETAIL_INDICATEUR>(); // Tableau des indicateurs

            if (User.IsInRole("partenaire"))
            {
                UTILISATEUR user = db.UTILISATEUR.Where(u => u.USERNAME == User.Identity.Name).FirstOrDefault();
                int id_organisation = user.ID_ORGANISATION;

                List<DONNEE_ET_INDICATEUR> liste_donnees_indicateurs = new List<DONNEE_ET_INDICATEUR>();
               // List<DATARESTRICTION> restrictions = db1.DATARESTRICTION.Where(d => d.ID_REGION == 13 && d.ID_ORGANIZATION == id_organisation).ToList();
               /* for (int i = 0; i < restrictions.Count; i++)
                {
                    int id_indicateur = restrictions[i].ID_INDICATEUR;
                    DETAIL_INDICATEUR indicateur = db.DETAIL_INDICATEUR.Where(d => d.ID_INDICATEUR == id_indicateur).FirstOrDefault();
                    DONNEE_ET_INDICATEUR donnee_indicateur = db.DONNEE_ET_INDICATEUR.Find(id_indicateur);

                    if (indicateur != null)
                        liste_indicateurs.Add(indicateur);

                    if (donnee_indicateur != null)
                        liste_donnees_indicateurs.Add(donnee_indicateur);
                }*/

                ViewBag.liste_indicateurs = liste_donnees_indicateurs; //Liste des indicateurs pour la partie des filtres
                string role = "partenaire";
                ViewBag.role = role;
            }

            else
            {
                ViewBag.liste_indicateurs = db.DONNEE_ET_INDICATEUR.Where(d => d.ID_REGION == 13).ToList(); //Liste des indicateurs pour la partie des filtres
                liste_indicateurs = db.DETAIL_INDICATEUR.Where(d => d.ID_REGION == 13).ToList();
                string role = "admin";
                ViewBag.role = role;
            }
            return View(liste_indicateurs);
        }

        public ActionResult IndexD(int? id, string mot)
        {
            ViewBag.domaines = db.DOMAINE.Where(d => d.ID_REGION == 13).ToList();
            ViewBag.themes = db.THEME.Where(d => d.ID_REGION == 13).ToList();
            ViewBag.soustheme = db.SOUS_THEME.Where(d => d.ID_REGION == 13).ToList();
            ViewBag.organisations = db.ORGANISATION.Where(o => o.ID_REGION == 13).ToList();
            ViewBag.recherche = mot;
            List<DETAIL_INDICATEUR> liste_indicateurs = new List<DETAIL_INDICATEUR>(); // Tableau des indicateurs

            if (User.IsInRole("partenaire"))
            {
                UTILISATEUR user = db.UTILISATEUR.Where(u => u.USERNAME == User.Identity.Name).FirstOrDefault();
                int id_organisation = user.ID_ORGANISATION;

                List<DONNEE_ET_INDICATEUR> liste_donnees_indicateurs = new List<DONNEE_ET_INDICATEUR>();
                List<DATARESTRICTION> restrictions = db1.DATARESTRICTION.Where(d => d.ID_REGION == 13 && d.ID_ORGANIZATION == id_organisation).ToList();
                for (int i = 0; i < restrictions.Count; i++)
                {
                    int id_indicateur = restrictions[i].ID_INDICATEUR;
                    DETAIL_INDICATEUR indicateur = db.DETAIL_INDICATEUR.Where(d => d.ID_INDICATEUR == id_indicateur).FirstOrDefault();
                    DONNEE_ET_INDICATEUR donnee_indicateur = db.DONNEE_ET_INDICATEUR.Find(id_indicateur);

                    if (indicateur != null)
                        liste_indicateurs.Add(indicateur);

                    if (donnee_indicateur != null)
                        liste_donnees_indicateurs.Add(donnee_indicateur);
                }

                ViewBag.liste_indicateurs = liste_donnees_indicateurs; //Liste des indicateurs pour la partie des filtres
                string role = "partenaire";
                ViewBag.role = role;
            }

            else
            {
                ViewBag.liste_indicateurs = db.DONNEE_ET_INDICATEUR.Where(d => d.ID_REGION == 13).ToList(); //Liste des indicateurs pour la partie des filtres
                liste_indicateurs = db.DETAIL_INDICATEUR.Where(d => d.ID_REGION == 13).ToList();
                string role = "admin";
                ViewBag.role = role;
            }
            return View(liste_indicateurs);
        }


        public ActionResult IndexP(int? id, string mot)
        {
            ViewBag.domaines = db.DOMAINE.Where(d => d.ID_REGION == 13).ToList();
            ViewBag.themes = db.THEME.Where(d => d.ID_REGION == 13).ToList();
            ViewBag.soustheme = db.SOUS_THEME.Where(d => d.ID_REGION == 13).ToList();
            ViewBag.organisations = db.ORGANISATION.Where(o => o.ID_REGION == 13).ToList();
            ViewBag.liste_indicateurs = db.DONNEE_ET_INDICATEUR.Where(d => d.ID_REGION == 13).ToList(); //Liste des indicateurs pour la partie des filtres
            ViewBag.recherche = mot;
            return View(db.DETAIL_INDICATEUR.Where(d => d.ID_REGION == 13).ToList());
        }

        // Liste des valeurs d'un indicateurs
        //[Authorize(Roles = "super-admin, admin , partenaire")]
        public ActionResult Listevaleurs(int? id, string mot, string type)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.indicateur = db.DONNEE_ET_INDICATEUR.Find(id);

            ViewBag.valeursregion = db1.VALEURS_TT_REGION.Where(v => v.ID_INDICATEUR == id).OrderBy(v => v.DATE_VALEUR).ToList();
            ViewBag.valeursprovinces = db1.VALEURS_TT_PROVINCES.Where(v => v.ID_INDICATEUR == id).OrderBy(v => v.DATE_VALEUR).ToList();
            ViewBag.valeurscommunes = db1.VALEURS_TT_COMMUNES.Where(v => v.ID_INDICATEUR == id).OrderBy(v => v.DATE_VALEUR).ToList();
            if(mot==null)
            {
                ViewBag.recherche = "";
            }
            else{
            ViewBag.recherche = mot;

            }
            ViewBag.typeIndCCODD = type;
            return View();
        }

        public ActionResult ListevaleursP(int? id, string mot, string type)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.indicateur = db.DONNEE_ET_INDICATEUR.Find(id);

            ViewBag.valeursregion = db1.VALEURS_TT_REGION.Where(v => v.ID_INDICATEUR == id && v.STATUT == "Validé").OrderBy(v => v.DATE_VALEUR).ToList();
            ViewBag.valeursprovinces = db1.VALEURS_TT_PROVINCES.Where(v => v.ID_INDICATEUR == id && v.STATUT == "Validé").OrderBy(v => v.DATE_VALEUR).ToList();
            ViewBag.valeurscommunes = db1.VALEURS_TT_COMMUNES.Where(v => v.ID_INDICATEUR == id && v.STATUT == "Validé").OrderBy(v => v.DATE_VALEUR).ToList();
            if (mot == null)
            {
                ViewBag.recherche = "";
            }
            else
            {
                ViewBag.recherche = mot;

            }
            ViewBag.typeIndCCODD = type;
            return View();
        }

        // GET: GererINDICATEUR/Details/5
        public ActionResult Details(int? id, string mot, string type)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DONNEE_ET_INDICATEUR dONNEE_ET_INDICATEUR = db.DONNEE_ET_INDICATEUR.Find(id);
            if (dONNEE_ET_INDICATEUR == null)
            {
                return HttpNotFound();
            }
            if (mot == null)
            {
                ViewBag.recherche = "";
            }
            else
            {
                ViewBag.recherche = mot;

            }
            ViewBag.typeIndCCODD = type;
            return View(dONNEE_ET_INDICATEUR);
        }

        public ActionResult DetailsP(int? id, string mot, string type)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DONNEE_ET_INDICATEUR dONNEE_ET_INDICATEUR = db.DONNEE_ET_INDICATEUR.Find(id);
            if (dONNEE_ET_INDICATEUR == null)
            {
                return HttpNotFound();
            }
            if (mot == null)
            {
                ViewBag.recherche = "";
            }
            else
            {
                ViewBag.recherche = mot;

            }
            ViewBag.typeIndCCODD = type;
            return View(dONNEE_ET_INDICATEUR);
        }

        public ActionResult Arboresence()
        {
            ViewBag.domaines = db.DOMAINE.Where(d => d.ID_REGION == 13).ToList();
            ViewBag.themes = db.THEME.Where(d => d.ID_REGION == 13).ToList();
            ViewBag.sousthemes = db.SOUS_THEME.Where(d => d.ID_REGION == 13).ToList();
            return View();
        }

        //06537650890
        //


        // GET: GererINDICATEUR/Create
        // [Authorize(Roles = "super-admin, admin")]
        public ActionResult Create(string controleur)
        {
            ViewBag.fournisseurs = db.ORGANISATION.Where(o => o.ID_REGION == 13).ToList();
            ViewBag.ID_FOURNISSEUR = new SelectList(db.ORGANISATION.Where(o => o.ID_REGION == 13), "ID_ORGANIZATION", "NOM_ORGANIZATION");
            // changement 
            ViewBag.objectifs = db.OBJECTIFDD.ToList();
            ViewBag.sousobjectifs = db.SOUS_OBJECTIFDD.ToList();
            // changement
           // ViewBag.sousobjectifs = db.SOUS_OBJECTIFDD.ToList();
            ViewBag.sousthemesCC = db.SOUS_THEME.Where(s => s.ID_REGION == 13 && s.ID_THEME == 1031).ToList();
            ViewBag.unites = db.UNITE.Where(u => u.ID_REGION == 13).ToList();
            ViewBag.domaines = db.DOMAINE.Where(d => d.ID_REGION == 13).ToList();
            ViewBag.themes = db.THEME.Where(t => t.ID_REGION == 13).ToList();
            ViewBag.sousthemes = db.SOUS_THEME.Where(s => s.ID_REGION == 13).ToList();
            ViewBag.ID_UNITE = new SelectList(db.UNITE.Where(u => u.ID_REGION == 13), "ID_UNITE", "LIBELLE_UNITE");


            ViewBag.controleur = controleur;

            return View();
        }

        // POST: GererINDICATEUR/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DONNEE_ET_INDICATEUR dONNEE_ET_INDICATEUR, string[] typeind, string controleur)
        {
            if (ModelState.IsValid)
            {
                //L'upload----------------------------
                HttpFileCollectionBase fichiers = Request.Files;
                if (fichiers.Count > 0)
                {
                    HttpPostedFileBase upld = fichiers[0];
                    if (upld != null && upld.ContentLength > 0)
                    {

                        string uploadFilesDir = Server.MapPath("~/App_Data/");
                        Directory.CreateDirectory(uploadFilesDir);
                        string fileSavePath = Path.Combine(uploadFilesDir, upld.FileName);
                        upld.SaveAs(fileSavePath);
                        dONNEE_ET_INDICATEUR.FICHIER_JOINT = fileSavePath;
                    }
                }

                if (dONNEE_ET_INDICATEUR.ID_SOUSTHEME == -1) {
                    ModelState.AddModelError("soustheme_vide", "If faut saisir un sous thème");
                    ViewBag.sousobjectifs = db.SOUS_OBJECTIFDD.ToList();
                    ViewBag.sousthemesCC = db.SOUS_THEME.Where(s => s.ID_REGION == 13 && s.ID_THEME == 51).ToList();
                    ViewBag.unites = db.UNITE.Where(u => u.ID_REGION == 13).ToList();
                    ViewBag.domaines = db.DOMAINE.Where(d => d.ID_REGION == 13).ToList();
                    ViewBag.themes = db.THEME.Where(t => t.ID_REGION == 13).ToList();
                    ViewBag.sousthemes = db.SOUS_THEME.Where(s => s.ID_REGION == 13).ToList();
                    ViewBag.ID_UNITE = new SelectList(db.UNITE.Where(u => u.ID_REGION == 13), "ID_UNITE", "LIBELLE_UNITE");
                    ViewBag.ID_FOURNISSEUR = new SelectList(db.ORGANISATION.Where(o => o.ID_REGION == 13), "ID_ORGANIZATION", "NOM_ORGANIZATION");
                    ViewBag.fournisseurs = db.ORGANISATION.Where(o => o.ID_REGION == 13).ToList();

                    return View(dONNEE_ET_INDICATEUR);
                }
                //----------------------------------
                int id = 1;
                if (db.DONNEE_ET_INDICATEUR.Count() > 0)
                {
                    id = db.DONNEE_ET_INDICATEUR.Max(us => us.ID_INDICATEUR) + 1;
                }
                for (int i = 0; i < typeind.Length; i++ )
                {
                    if (typeind[i] == "CC")
                    {

                        dONNEE_ET_INDICATEUR.IS_CC = true;
                      /*  dONNEE_ET_INDICATEUR.IS_ODD = false;
                        dONNEE_ET_INDICATEUR.IS_DD = false;
                        dONNEE_ET_INDICATEUR.ID_SOUSOBJECTIF = null;*/
                    }
                    if (typeind[i] == "ODD")
                    {
                        dONNEE_ET_INDICATEUR.IS_ODD = true;
                       /* dONNEE_ET_INDICATEUR.IS_DD = false;
                        dONNEE_ET_INDICATEUR.IS_CC = false;*/
                    }
                    if (typeind[i] == "SDD")
                    {
                        dONNEE_ET_INDICATEUR.IS_DD = true;
                       /* dONNEE_ET_INDICATEUR.IS_ODD = false;
                        dONNEE_ET_INDICATEUR.IS_CC = false;
                        dONNEE_ET_INDICATEUR.ID_SOUSOBJECTIF = null;*/
                    }
                }
              
                dONNEE_ET_INDICATEUR.ID_INDICATEUR = id;
                dONNEE_ET_INDICATEUR.ID_REGION = 13;
                dONNEE_ET_INDICATEUR.DATE_CREATION = DateTime.Now;
                if (dONNEE_ET_INDICATEUR.ID_UNITE == -1)
                    dONNEE_ET_INDICATEUR.ID_UNITE = null;
                if (dONNEE_ET_INDICATEUR.ID_FOURNISSEUR == -1)
                    dONNEE_ET_INDICATEUR.ID_FOURNISSEUR = null;
                db.DONNEE_ET_INDICATEUR.Add(dONNEE_ET_INDICATEUR);
                db.SaveChanges();


               /* if (controleur == "CC")
                    return RedirectToAction("Index", "CC");

                else if (controleur == "ODD")
                    return RedirectToAction("Index", "ODD");*/

                return RedirectToAction("Index");
            }

            ViewBag.sousobjectifs = db.SOUS_OBJECTIFDD.ToList();
            ViewBag.sousthemesCC = db.SOUS_THEME.Where(s => s.ID_REGION == 13 && s.ID_THEME == 51).ToList();
            ViewBag.unites = db.UNITE.Where(u => u.ID_REGION == 13).ToList();
            ViewBag.domaines = db.DOMAINE.Where(d => d.ID_REGION == 13).ToList();
            ViewBag.themes = db.THEME.Where(t => t.ID_REGION == 13).ToList();
            ViewBag.sousthemes = db.SOUS_THEME.Where(s => s.ID_REGION == 13).ToList();
            ViewBag.ID_UNITE = new SelectList(db.UNITE.Where(u => u.ID_REGION == 13), "ID_UNITE", "LIBELLE_UNITE");
            ViewBag.ID_FOURNISSEUR = new SelectList(db.ORGANISATION.Where(o => o.ID_REGION == 13), "ID_ORGANIZATION", "NOM_ORGANIZATION", dONNEE_ET_INDICATEUR.ID_FOURNISSEUR);
            ViewBag.fournisseurs = db.ORGANISATION.Where(o => o.ID_REGION == 13).ToList();

            return View(dONNEE_ET_INDICATEUR);
        }

        // GET: GererINDICATEUR/Edit/5
       // [Authorize(Roles = "super-admin, admin")]
        public ActionResult Edit(int? id, string mot, string type, string controleur)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DONNEE_ET_INDICATEUR dONNEE_ET_INDICATEUR = db.DONNEE_ET_INDICATEUR.Find(id);
            if (dONNEE_ET_INDICATEUR == null)
            {
                return HttpNotFound();
            }
            ViewBag.fournisseurs = db.ORGANISATION.Where(o => o.ID_REGION == 13).ToList();
          //  ViewBag.sousobjectifs = db.SOUS_OBJECTIFDD.ToList();
            ViewBag.ID_FOURNISSEUR = new SelectList(db.ORGANISATION.Where(u => u.ID_REGION == 13), "ID_ORGANIZATION", "NOM_ORGANIZATION", dONNEE_ET_INDICATEUR.ID_FOURNISSEUR);
            ViewBag.ID_SOUSTHEME = new SelectList(db.SOUS_THEME.Where(u => u.ID_REGION == 13), "ID_SOUSTHEME", "LIBELLE_SOUSTHEME", dONNEE_ET_INDICATEUR.ID_SOUSTHEME);
            //ViewBag.ID_SOUSOBJECTIF = new SelectList(db.SOUS_OBJECTIFDD.Where(u => u.ID_REGION == 13), "ID_SOUSOBJECTIF", "LIBELLE_SOUSOBJECTIF", dONNEE_ET_INDICATEUR.ID_SOUSOBJECTIF);
            ViewBag.ID_UNITE = new SelectList(db.UNITE.Where(u => u.ID_REGION == 13), "ID_UNITE", "LIBELLE_UNITE", dONNEE_ET_INDICATEUR.ID_UNITE);
            ViewBag.sousthemesCC = db.SOUS_THEME.Where(s => s.ID_REGION == 13 && s.ID_THEME == 1031).ToList();

            ViewBag.CC = dONNEE_ET_INDICATEUR.IS_CC;
            ViewBag.ODD = dONNEE_ET_INDICATEUR.IS_ODD;
            ViewBag.SNDD = dONNEE_ET_INDICATEUR.IS_DD;


            //<!--10/07/2018-->
           ViewBag.objectifs = db.OBJECTIFDD.ToList();
            int id_objectif = -1;
            if (dONNEE_ET_INDICATEUR.ID_SOUSOBJECTIF != -1)
            {
                SOUS_OBJECTIFDD sous_objectif = db.SOUS_OBJECTIFDD.Find(dONNEE_ET_INDICATEUR.ID_SOUSOBJECTIF);
                id_objectif = sous_objectif.ID_OBJECTIF;
            }

            ViewBag.objectifSelect = id_objectif;
            ViewBag.sousobjectifs = db.SOUS_OBJECTIFDD.Where(so => so.ID_OBJECTIF == id_objectif).ToList();
            ViewBag.sousObjectifSelect = dONNEE_ET_INDICATEUR.ID_SOUSOBJECTIF;
            //<!--10/07/2018-->
            ViewBag.composanteCCSelect = dONNEE_ET_INDICATEUR.ID_SOUSCATEGORIECC;
            
            ViewBag.controleur = controleur;
            ViewBag.recherche = mot;
            ViewBag.typeIndCCODD = type;
            /////
            ViewBag.typeIndicateur = dONNEE_ET_INDICATEUR.TYPE;
            ViewBag.dpsir = dONNEE_ET_INDICATEUR.CATEGORIEDPSIR;
            ViewBag.Priorite = dONNEE_ET_INDICATEUR.PRIORITE;
            ViewBag.Disponibilite = dONNEE_ET_INDICATEUR.PAYANT;
            /////
            return View(dONNEE_ET_INDICATEUR);
        }

        // POST: GererINDICATEUR/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DONNEE_ET_INDICATEUR dONNEE_ET_INDICATEUR, HttpPostedFileBase upload, string[] typeind, string controleur, string mot)
        {
            if (ModelState.IsValid)
            {
                //L'upload----------------------------
                HttpFileCollectionBase fichiers = Request.Files;
                if (fichiers.Count > 0)
                {
                    HttpPostedFileBase upld = fichiers[0];
                    if (upld != null && upld.ContentLength > 0)
                    {

                        string uploadFilesDir = Server.MapPath("~/App_Data/");
                        Directory.CreateDirectory(uploadFilesDir);
                        string fileSavePath = Path.Combine(uploadFilesDir, upld.FileName);
                        upld.SaveAs(fileSavePath);
                        dONNEE_ET_INDICATEUR.FICHIER_JOINT = fileSavePath;
                    }
                }
                //----------------------------------
                DONNEE_ET_INDICATEUR ind = db.DONNEE_ET_INDICATEUR.Where(i => i.ID_INDICATEUR == dONNEE_ET_INDICATEUR.ID_INDICATEUR).FirstOrDefault();
                if (ind != null)
                {
                    db.Entry(ind).State = EntityState.Detached;
                }
                for (int i = 0; i < typeind.Length; i++)
                {
                    if (typeind[i] == "CC")
                    {

                        dONNEE_ET_INDICATEUR.IS_CC = true;
                     
                    }
                    if (typeind[i] == "ODD")
                    {
                        dONNEE_ET_INDICATEUR.IS_ODD = true;
                    
                    }
                    if (typeind[i] == "SDD")
                    {
                        dONNEE_ET_INDICATEUR.IS_DD = true;
                     
                    }
                }
                db.Entry(dONNEE_ET_INDICATEUR).State = EntityState.Modified;
                db.SaveChanges();


                if (controleur == "CC")
                    return RedirectToAction("Index", "CC", new { @mot = mot });

                else if (controleur == "ODD")
                    return RedirectToAction("Index", "ODD", new { @mot = mot });

                return RedirectToAction("Index" , new {@mot=mot});
            }
            //<!--10/07/2018-->
            ViewBag.objectifs = db.OBJECTIFDD.ToList();
            int id_objectif = -1;
            if (dONNEE_ET_INDICATEUR.ID_SOUSOBJECTIF != -1)
            {
                SOUS_OBJECTIFDD sous_objectif = db.SOUS_OBJECTIFDD.Find(dONNEE_ET_INDICATEUR.ID_SOUSOBJECTIF);
                id_objectif = sous_objectif.ID_OBJECTIF;
            }


            ViewBag.objectifSelect = id_objectif;
            ViewBag.sousobjectifs = db.SOUS_OBJECTIFDD.Where(so => so.ID_OBJECTIF == id_objectif).ToList();
            ViewBag.sousObjectifSelect = dONNEE_ET_INDICATEUR.ID_SOUSOBJECTIF;
            //<!--10/07/2018-->
       //     ViewBag.sousobjectifs = db.SOUS_OBJECTIFDD.ToList();
            ViewBag.ID_SOUSTHEME = new SelectList(db.SOUS_THEME.Where(u => u.ID_REGION == 13), "ID_SOUSTHEME", "LIBELLE_SOUSTHEME", dONNEE_ET_INDICATEUR.ID_SOUSTHEME);
            ViewBag.ID_SOUSOBJECTIF = new SelectList(db.SOUS_OBJECTIFDD.Where(u => u.ID_REGION == 13), "ID_SOUSOBJECTIF", "LIBELLE_SOUSOBJECTIF", dONNEE_ET_INDICATEUR.ID_SOUSOBJECTIF);
            ViewBag.ID_UNITE = new SelectList(db.UNITE.Where(u => u.ID_REGION == 13), "ID_UNITE", "LIBELLE_UNITE", dONNEE_ET_INDICATEUR.ID_UNITE);
            ViewBag.ID_FOURNISSEUR = new SelectList(db.ORGANISATION.Where(u => u.ID_REGION == 13), "ID_ORGANIZATION", "NOM_ORGANIZATION", dONNEE_ET_INDICATEUR.ID_FOURNISSEUR);

            return View(dONNEE_ET_INDICATEUR);
        }

        // GET: GererINDICATEUR/Delete/5
        //[Authorize(Roles = "super-admin, admin")]
        public ActionResult Delete(int? id, string controleur, string mot, string type)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DONNEE_ET_INDICATEUR dONNEE_ET_INDICATEUR = db.DONNEE_ET_INDICATEUR.Find(id);
            if (dONNEE_ET_INDICATEUR == null)
            {
                return HttpNotFound();
            }

            ViewBag.recherche = mot;
            ViewBag.typeIndCCODD = type;
            ViewBag.controleur = controleur;
            return View(dONNEE_ET_INDICATEUR);
        }

        // POST: GererINDICATEUR/Delete/5
       /* [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, string controleur)
        {
            DONNEE_ET_INDICATEUR dONNEE_ET_INDICATEUR = db.DONNEE_ET_INDICATEUR.Find(id);
            List<VALEURS> valeurs = new List<VALEURS>();
            valeurs = db.VALEURS.Where(o => o.ID_INDICATEUR == id).ToList();
            foreach (VALEURS v in valeurs)
            {
                db.VALEURS.Remove(v);
            }

            List<VALIDATION> vald = new List<VALIDATION>();
            vald = db1.VALIDATION.Where(o => o.id_indicateur == id).ToList();
            foreach (VALIDATION v in vald)
            {
                db1.VALIDATION.Remove(v);
            }


            List<INDICATEURS_TBS_ODD> IndTBS = new List<INDICATEURS_TBS_ODD>();
            IndTBS = db1.INDICATEURS_TBS_ODD.Where(o => o.ID_INDICATEUR == id).ToList();
            foreach (INDICATEURS_TBS_ODD v in IndTBS)
            {
                db1.INDICATEURS_TBS_ODD.Remove(v);
            }

            List<INDICATEURS_TBS_CC> IndTBS1 = new List<INDICATEURS_TBS_CC>();
            IndTBS1 = db.INDICATEURS_TBS_CC.Where(o => o.ID_INDICATEUR == id).ToList();
            foreach (INDICATEURS_TBS_CC v in IndTBS1)
            {
                db.INDICATEURS_TBS_CC.Remove(v);
            }

            List<INDICATEURS_TBS> IndTBS2 = new List<INDICATEURS_TBS>();
            IndTBS2 = db.INDICATEURS_TBS.Where(o => o.ID_INDICATEUR == id).ToList();
            foreach (INDICATEURS_TBS v in IndTBS2)
            {
                db.INDICATEURS_TBS.Remove(v);
            }

            //Suppression de la restriction
           RESTRICTIONLISTE restriction = db1.RESTRICTIONLISTE.Where(t => t.id_indicateur == id).FirstOrDefault();
           if (restriction != null)
            db1.RESTRICTIONLISTE.Remove(restriction); 

            db1.SaveChanges();
            db.DONNEE_ET_INDICATEUR.Remove(dONNEE_ET_INDICATEUR);
            db.SaveChanges();

            if (controleur == "CC")
                return RedirectToAction("Index", "CC");

            else if (controleur == "ODD")
                return RedirectToAction("Index", "ODD");

            return RedirectToAction("Index");
        }*/
        // POST: GererINDICATEUR/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, string controleur, string mot)
        {
            DONNEE_ET_INDICATEUR dONNEE_ET_INDICATEUR = db.DONNEE_ET_INDICATEUR.Find(id);
          
            db.DONNEE_ET_INDICATEUR.Remove(dONNEE_ET_INDICATEUR);
            db.SaveChanges();

            if (controleur == "CC")
                return RedirectToAction("Index", "CC", new { @mot = mot });

            else if (controleur == "ODD")
                return RedirectToAction("Index", "ODD", new { @mot = mot });
            
            return RedirectToAction("Index", new { @mot = mot });
        }
        // Les actions pour  ajouter des méthodes de saisie des valeurs

        public ActionResult Valeurs(int? id, string mot, string type)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DONNEE_ET_INDICATEUR dONNEE_ET_INDICATEUR = db.DONNEE_ET_INDICATEUR.Where(i => i.ID_INDICATEUR == id && i.ID_REGION == 13).FirstOrDefault(); // db.DONNEE_ET_INDICATEUR.Find(id);
            if (dONNEE_ET_INDICATEUR == null)
            {
                return HttpNotFound();
            }
            //MyMembershipProvider currentUser;

            //string username = currentUser.GetUser(User.Identity.Name,true); //** get UserName
            //Guid userGuid = (Guid)Membership.GetUser().ProviderUserKey; //** get user ID
            ViewBag.idusertst = db.UTILISATEUR.Where(x => x.USERNAME == User.Identity.Name.ToString()).FirstOrDefault().ID_UTILISATEUR;
            //User.Identity.Name.ToString();
            ViewBag.regions = db.REGIONS.ToList();
            ViewBag.provinces = db.PROVINCES.Where(o => o.id_region == 13).ToList();
            var id_prov = (from p in db.PROVINCES where p.id_region == 13 select p.code_prov).ToList();
            ViewBag.communes = db.COMMUNES.Where(x => id_prov.Contains(x.code_prov)).ToList(); // db.COMMUNES.ToList(); //Where(o => o.ID_REGION == '2').
            ViewBag.domaines = db.DOMAINE.Where(d => d.ID_REGION == 13).ToList();
            ViewBag.themes = db.THEME.Where(d => d.ID_REGION == 13).ToList();
            ViewBag.sousthemes = db.SOUS_THEME.Where(d => d.ID_REGION == 13).ToList();
            ViewBag.id_indicateur = id;
            ViewBag.indicateur = new SelectList(db.DONNEE_ET_INDICATEUR.Where(i => i.ID_INDICATEUR == id && i.ID_REGION == 13), "ID_INDICATEUR", "LIBELLE_INDICATEUR", dONNEE_ET_INDICATEUR.ID_INDICATEUR);
            ViewBag.recherche = mot;
            ViewBag.typeIndCCODD = type;
            return View(); // (dONNEE_ET_INDICATEUR);
        }

        public JsonResult ValeursP(List<DataValeur> valeurs)
        {

            if (valeurs != null)
            {
                if (valeurs[0].type_portee == "REGION")
                {
                    int id = (db.VALEURS.Count() > 0) ? db.VALEURS.Max(us => us.ID_VALEUR) + 1 : 1;
                    PORTEE por = db.PORTEE.Where(p => p.NIVEAU_GEO == "REGION" && p.ZONE_GEO == 13).FirstOrDefault();
                    if (por != null)
                    {
                        VALEURS val = new VALEURS
                        {
                            ID_VALEUR = id,
                            VALEUR = valeurs[0].valeur,
                            DATE_SAISIE = DateTime.Now,
                            DATE_VALEUR = valeurs[0].date_valeur,
                            STATUT = valeurs[0].statut, // "Non validé",
                            OBSERVATION = valeurs[0].observation, // "Aucune",
                            ID_PORTEE = por.ID_PORTEE,
                            ID_INDICATEUR = valeurs[0].indicateur
                        };

                        db.VALEURS.Add(val);
                        db.SaveChanges();

                        VALIDATION VD = new VALIDATION
                        {
                            id_indicateur = valeurs[0].indicateur,
                            id_valeur = id,
                            id_user = db.UTILISATEUR.Where(x => x.USERNAME == User.Identity.Name.ToString()).FirstOrDefault().ID_UTILISATEUR
                        };

                        db1.VALIDATION.Add(VD);
                        db1.SaveChanges();

                    }
                }

                else if (valeurs[0].type_portee == "PROVINCE")
                {
                    int id = (db.VALEURS.Count() > 0) ? db.VALEURS.Max(us => us.ID_VALEUR) + 1 : 1;
                    foreach (DataValeur valeur in valeurs)
                    {
                        PORTEE por = db.PORTEE.Where(p => p.NIVEAU_GEO == "PROVINCE" && p.ZONE_GEO == valeur.nom_porteeP).FirstOrDefault();
                        if (por != null)
                        {
                            VALEURS val = new VALEURS
                            {
                                ID_VALEUR = id,
                                VALEUR = valeur.valeur,
                                DATE_SAISIE = DateTime.Now,
                                DATE_VALEUR = valeur.date_valeur,
                                STATUT = valeur.statut, // "Non validé",
                                OBSERVATION = valeur.observation, // "Aucune",
                                ID_PORTEE = por.ID_PORTEE,
                                ID_INDICATEUR = valeur.indicateur
                            };

                            db.VALEURS.Add(val);
                            db.SaveChanges();

                            VALIDATION VD = new VALIDATION
                            {
                                id_indicateur = valeur.indicateur,
                                id_valeur = id,
                                id_user = db.UTILISATEUR.Where(x => x.USERNAME == User.Identity.Name.ToString()).FirstOrDefault().ID_UTILISATEUR
                            };

                            db1.VALIDATION.Add(VD);
                            db1.SaveChanges();

                            id++;
                        }


                    }
                }

                else
                {
                    int id = (db.VALEURS.Count() > 0) ? db.VALEURS.Max(us => us.ID_VALEUR) + 1 : 1;
                    foreach (DataValeur valeur in valeurs)
                    {
                        PORTEE por = db.PORTEE.Where(p => p.NIVEAU_GEO == "COMMUNE" && p.ZONE_GEO == valeur.nom_porteeC).FirstOrDefault();
                        if (por != null)
                        {
                            VALEURS val = new VALEURS
                            {
                                ID_VALEUR = id,
                                VALEUR = valeur.valeur,
                                DATE_SAISIE = DateTime.Now,
                                DATE_VALEUR = valeur.date_valeur,
                                STATUT = valeur.statut, // "Non validé",
                                OBSERVATION = valeur.observation, // "Aucune",
                                ID_PORTEE = por.ID_PORTEE,
                                ID_INDICATEUR = valeur.indicateur
                            };

                            db.VALEURS.Add(val);
                            db.SaveChanges();

                            VALIDATION VD = new VALIDATION
                            {
                                id_indicateur = valeur.indicateur,
                                id_valeur = id,
                                id_user = db.UTILISATEUR.Where(x => x.USERNAME == User.Identity.Name.ToString()).FirstOrDefault().ID_UTILISATEUR
                            };

                            db1.VALIDATION.Add(VD);
                            db1.SaveChanges();
                            id++;
                        }

                    }
                }
                return Json("Success");
            }
            return Json(null);
        }

        public JsonResult getProvincesRegion(int id_region)
        {
            var provinces = db.PROVINCES.Where(p => p.id_region == id_region)
                .Select(p => new
                {
                    p.code_prov,
                    p.Province_N
                });

            return Json(provinces);
        }

        public JsonResult getCommunesProvince(int code_prov)
        {
            var communes = db.COMMUNES.Where(c => c.code_prov == code_prov)
                .Select(c => new
                {
                    c.id_commune,
                    c.Commune_Na,
                    c.Province_N,
                    c.code_prov
                });

            return Json(communes);
        }

        //
        public ActionResult ValeursInd(int? id)
        {
            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                ViewBag.indicateur = new SelectList(db.DONNEE_ET_INDICATEUR.Where(i => i.ID_REGION == 13), "ID_INDICATEUR", "LIBELLE_INDICATEUR");

            }
            else
            {
                DONNEE_ET_INDICATEUR dONNEE_ET_INDICATEUR = db.DONNEE_ET_INDICATEUR.Where(i => i.ID_INDICATEUR == id && i.ID_REGION == 13).FirstOrDefault(); // db.DONNEE_ET_INDICATEUR.Find(id);
                if (dONNEE_ET_INDICATEUR == null)
                {
                    return HttpNotFound();
                }
                ViewBag.indicateur = new SelectList(db.DONNEE_ET_INDICATEUR.Where(i => i.ID_INDICATEUR == id && i.ID_REGION == 13), "ID_INDICATEUR", "LIBELLE_INDICATEUR", dONNEE_ET_INDICATEUR.ID_INDICATEUR);
            }
            ViewBag.regions = db.REGIONS.ToList();
            ViewBag.provinces = db.PROVINCES.Where(o => o.id_region == 13).ToList();
            var id_prov = (from p in db.PROVINCES where p.id_region == 13 select p.code_prov).ToList();
            ViewBag.communes = db.COMMUNES.Where(x => id_prov.Contains(x.code_prov)).ToList(); // db.COMMUNES.ToList(); //Where(o => o.ID_REGION == '2').
            ViewBag.domaines = db.DOMAINE.Where(d => d.ID_REGION == 13).ToList();
            ViewBag.themes = db.THEME.Where(d => d.ID_REGION == 13).ToList();
            ViewBag.sousthemes = db.SOUS_THEME.Where(d => d.ID_REGION == 13).ToList();
            return View(); // (dONNEE_ET_INDICATEUR);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ValeursInd(DataValeur data)
        {
            if (ModelState.IsValid)
            {
                int id = 1;
                if (db.VALEURS.Count() > 0)
                {
                    id = db.VALEURS.Max(us => us.ID_VALEUR) + 1;
                }
                PORTEE por = new PORTEE();
                if (data.type_portee == "REGION")
                {
                    por = db.PORTEE.Where(p => p.NIVEAU_GEO == "REGION" && p.ZONE_GEO == 2).FirstOrDefault();
                }
                if (data.type_portee == "PROVINCE")
                {
                    //por = db.PORTEE.Where(p => p.NIVEAU_GEO == "PROVINCE" && p.ZONE_GEO == 27).FirstOrDefault();
                    por = db.PORTEE.Where(p => p.NIVEAU_GEO == "PROVINCE" && p.ZONE_GEO == data.nom_porteeP).FirstOrDefault();

                }
                if (data.type_portee == "COMMUNE")
                {
                    por = db.PORTEE.Where(p => p.NIVEAU_GEO == "COMMUNE" && p.ZONE_GEO == data.nom_porteeC).FirstOrDefault();
                }
                if (por != null)
                {
                    VALEURS val = new VALEURS
                    {
                        ID_VALEUR = id,
                        VALEUR = data.valeur,
                        DATE_SAISIE = DateTime.Now,
                        DATE_VALEUR = data.date_valeur,
                        STATUT = data.statut, // "Non validé",
                        OBSERVATION = data.observation, // "Aucune",
                        ID_PORTEE = por.ID_PORTEE,
                        ID_INDICATEUR = data.indicateur
                    };

                    db.VALEURS.Add(val);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            return View(data);

        }

        // POST: GererINDICATEUR/Valeurs
        //[HttpPost, ActionName("Valeurs")]
        // [ValidateAntiForgeryToken]
        public ActionResult ValeursSaisie()
        {
            return RedirectToAction("Index");
        }


        public JsonResult validerValeur(int id_valeur)
        {
            VALEURS valeur = db.VALEURS.Find(id_valeur);
            if (valeur != null)
            {
                valeur.STATUT = "Validé";

                VALIDATION validation = db1.VALIDATION.Where(v => v.id_valeur == id_valeur).FirstOrDefault();
                db1.VALIDATION.Remove(validation);

                db.SaveChanges();
                db1.SaveChanges();

                return Json(true);
            }

            return Json(null);
        }


        public JsonResult supprimerValeur(int id_valeur)
        {
            VALEURS valeur = db.VALEURS.Find(id_valeur);
            if (valeur != null)
            {
                VALIDATION validation = db1.VALIDATION.Where(v => v.id_valeur == id_valeur).FirstOrDefault();
                db1.VALIDATION.Remove(validation);
                db.VALEURS.Remove(valeur);

                db.SaveChanges();
                db1.SaveChanges();

                return Json(true);
            }

            return Json(null);
        }

        public ActionResult downloadFile(string chemin)
        {
            if (System.IO.File.Exists(chemin))
            {
                byte[] bytes = System.IO.File.ReadAllBytes(chemin);
                string contentType = MimeMapping.GetMimeMapping(chemin);
                FileContentResult contenu = new FileContentResult(bytes, contentType);
                string[] parts = chemin.Split('\\');
                contenu.FileDownloadName = parts[parts.Length - 1];
                return contenu;
            }

            return HttpNotFound();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
