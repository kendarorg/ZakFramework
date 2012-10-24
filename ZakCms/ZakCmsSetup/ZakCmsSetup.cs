#define MAXIMAL
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Resources;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ZakCms.Factories;
using ZakCms.Models.Entitites;
using ZakCms.Repositories;
using ZakCmsSetup.Properties;
using ZakCmsSetup.Utils;
using ZakWeb.Utils.Commons;

namespace ZakCmsSetup
{
	public partial class ZakCmsSetup : Form
	{
		public ZakCmsSetup()
		{
			InitializeComponent();
		}

		private void ZakCmsSetupLoad(object sender, EventArgs e)
		{
			tbDbConnectionString.Text = ConfigurationManager.ConnectionStrings["CmsDb"].ConnectionString;
			try
			{
				if (AppDomain.CurrentDomain.GetData("DataDirectory") != null)
					tbDbDataDirectory.Text = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
			}
			catch (Exception)
			{
				Debug.WriteLine("Useless error");
			}
		}

		private void BtRunSetupClick(object sender, EventArgs e)
		{
			SetupConnectionString();
			if (cbDropTables.Checked)
			{
				ExecuteScript("TablesDrop.sql");
			}
			if (cbCreateTables.Checked)
			{
				ExecuteScript("TablesCreate.sql");
			}
			if (cbEmptyTables.Checked)
			{
				ExecuteScript("TablesTruncate.sql");
			}
			if (cbFillTables.Checked)
			{
				FillTables();
			}
			MessageBox.Show(Resources.completed_message);
		}

		private void SetupConnectionString()
		{
			var dataDirectory = ConfigurationManager.AppSettings["DataDirectory"];
			if (!string.IsNullOrWhiteSpace(tbDbDataDirectory.Text))
			{
				dataDirectory = tbDbDataDirectory.Text.Trim();
			}
			string path = Application.ExecutablePath;
			var absoluteDataDirectory = Path.Combine(path, dataDirectory);
			var fullPAt = Path.GetFullPath(absoluteDataDirectory);
			AppDomain.CurrentDomain.SetData("DataDirectory", fullPAt);

			SimpleFakeFactory.InitializeSimpleFakeFactory(true, true, fullPAt);
		}

		private void ExecuteScript(string scriptFile)
		{
			string sqlConnectionString = tbDbConnectionString.Text.Trim();
			var fileContent = ResourcesReader.LoadManifestResourceString(scriptFile, this);

			IEnumerable<string> commandStrings = Regex.Split(fileContent, @"^\s*GO\s*$",
			                                                 RegexOptions.Multiline | RegexOptions.IgnoreCase);

			var conn = new SqlConnection(sqlConnectionString);
			conn.Open();
			foreach (var line in commandStrings)
			{
				if (!string.IsNullOrWhiteSpace(line))
				{
					new SqlCommand(line, conn).ExecuteNonQuery();
				}
			}

			conn.Close();
		}

		private List<Int64> _tags;

		private void FillTables()
		{
			_tags = new List<long>();
			var tag = SimpleFakeFactory.Create<ITagsRepository>();
			_tags.Add(tag.Create(new TagModel {Code = "projects", Description = "Projects"}));
			_tags.Add(tag.Create(new TagModel {Code = "home", Description = "Home"}));
			_tags.Add(tag.Create(new TagModel {Code = "news", Description = "News"}));
#if MAXIMAL
			_tags.Add(tag.Create(new TagModel {Code = "help", Description = "Help"}));
			_tags.Add(tag.Create(new TagModel {Code = "people", Description = "People"}));
			_tags.Add(tag.Create(new TagModel {Code = "who", Description = "Who"}));
			_tags.Add(tag.Create(new TagModel {Code = "contacts", Description = "Contacts"}));
#endif

			var lan = SimpleFakeFactory.Create<ILanguagesRepository>();
			var defaultLanguage = lan.Create(new LanguageModel {Code = "en-us", Description = "English"});
#if MAXIMAL
			lan.Create(new LanguageModel {Code = "it-it", Description = "Italiano"});
			lan.Create(new LanguageModel {Code = "fr-fr", Description = "Français"});
			lan.Create(new LanguageModel {Code = "defau", Description = "Default"});
#endif
			var use = SimpleFakeFactory.Create<IBEUsersRepository>();
			use.Create(new UserModel {UserId = "admin", UserPassword = "admin"});
			use.Create(new UserModel {UserId = "guest", UserPassword = "guest"});

			var usf = SimpleFakeFactory.Create<IFEUsersRepository>();
			usf.Create(new UserModel {UserId = "user01", UserPassword = "user01"});
			usf.Create(new UserModel {UserId = "user02", UserPassword = "user02"});

			var cmp = SimpleFakeFactory.Create<ICompaniesRepository>();
			cmp.Create(new CompanyModel {Code = "north", Description = "Northwind"});
#if MAXIMAL
			cmp.Create(new CompanyModel {Code = "adven", Description = "AdventureWorks"});
			cmp.Create(new CompanyModel {Code = "codes", Description = "Codes"});
#endif

			foreach (var company in cmp.GetAll())
			{
				var cm = company as CompanyModel;
				if (cm != null && cm.Code != "codes")
				{
					foreach (var language in lan.GetAll())
					{
						var la = language as LanguageModel;
						//First setup feeds
						SetupFeed(cm, la, "news", "projects");
						//Then contents so that some feed will be automagically filled
						SetupWebSite(cm, la);
					}
				}
				else
				{
#if MAXIMAL
					SetupCodeWebsite(cm, defaultLanguage);
#endif
				}
			}
		}

		private void SetupFeed(CompanyModel cm, LanguageModel la, params string[] tags)
		{
			foreach (var tag in tags)
			{
				SetupSingleFeed(cm, la, tag);
			}
		}

		private void SetupSingleFeed(CompanyModel cm, LanguageModel la, string tag)
		{
			var fee = SimpleFakeFactory.Create<IFeedsRepository>();
			var fet = SimpleFakeFactory.Create<IFeedsToTagsRepository>();
			var ter = SimpleFakeFactory.Create<ITagsRepository>();
			var fm = new FeedModel
				{
					Company = cm,
					Language = la,
					Title = cm.Description + " " + tag
				};
			var feed = fee.Create(fm);
			var tagel = (TagModel) ter.GetByTagCode(tag);
			fet.Create(feed, tagel.Id);
		}

		private void SetupWebSite(CompanyModel cm, LanguageModel la)
		{
			string lang = la.Code.Trim().Replace("-", "_").ToLowerInvariant();
			Int64 root = CreateArticle(cm, la, lang, "home", null, 0);
			CreateArticle(cm, la, lang, "who", null, root);
			CreateArticle(cm, la, lang, "projects", null, root);
			CreateArticle(cm, la, lang, "contactus", null, root);
#if MAXIMAL
			CreateArticle(cm, la, lang, "restricted", null, root, true);
#endif
		}

		private Int64 CreateArticle(CompanyModel cm, LanguageModel la, string lang,
		                            string titleResource, string content, Int64 parentId, bool restricted = false)
		{
			var tgr = SimpleFakeFactory.Create<ITagsRepository>();
			var atr = SimpleFakeFactory.Create<IArticlesToTagsRepository>();
			var tag = (TagModel) tgr.GetByTagCode(titleResource);

			titleResource = lang + "_" + titleResource;
			var rm = new ResourceManager("ZakCmsSetup.Properties.Resources", GetType().Assembly);
			var art = SimpleFakeFactory.Create<IArticlesRepository>();
			if (string.IsNullOrWhiteSpace(content))
			{
				content = rm.GetString(titleResource) + " Content for company " + cm.Description + " in " + la.Description;
			}
			var title = rm.GetString(titleResource) + "-" + cm.Description;
			var articleId = art.Create(new ArticleModel
				{
					Language = la,
					Company = cm,
					Title = title,
					SeoTitle = SeoUtils.BuildSeoFriendlyName(title),
					Content = content,
					ParentId = parentId,
					IsAuthenticated = restricted
				});

			if (tag != null) atr.Create(articleId, tag.Id);
			return articleId;
		}

		private void SetupCodeWebsite(CompanyModel cm, long language)
		{
			var art = SimpleFakeFactory.Create<IArticlesRepository>();
			var tag = SimpleFakeFactory.Create<IArticlesToTagsRepository>();
			var homePage = art.Create(new ArticleModel
				{
					Language = new LanguageModel(language),
					Company = cm,
					Title = "Home Page",
					Content = "Code documentation",
					ParentId = 0
				});
			tag.Create(homePage, _tags[0]);
			tag.Create(homePage, _tags[1]);
			tag.Create(homePage, _tags[2]);


			var pg = art.Create(new ArticleModel
				{
					Language = new LanguageModel(language),
					Company = cm,
					Title = "Complex Page",
					Content = ResourcesReader.LoadManifestResourceString("ComplexPage.txt", this),
					ParentId = homePage
				});
			tag.Create(pg, _tags[0]);
			tag.Create(pg, _tags[1]);


			pg = art.Create(new ArticleModel
				{
					Language = new LanguageModel(language),
					Company = cm,
					Title = "Page With Internal Items",
					Content = ResourcesReader.LoadManifestResourceString("PageWithInternalItems.txt", this),
					ParentId = homePage
				});
			tag.Create(pg, _tags[1]);
			tag.Create(pg, _tags[2]);

			pg = art.Create(new ArticleModel
				{
					Language = new LanguageModel(language),
					Company = cm,
					Title = "Page With Links",
					Content = ResourcesReader.LoadManifestResourceString("PageWithLinks.txt", this),
					ParentId = homePage
				});
			tag.Create(pg, _tags[0]);
			tag.Create(pg, _tags[2]);

			pg = art.Create(new ArticleModel
				{
					Language = new LanguageModel(language),
					Company = cm,
					Title = "Page With Table",
					Content = ResourcesReader.LoadManifestResourceString("PageWithTable.txt", this),
					ParentId = homePage
				});
			tag.Create(pg, _tags[0]);

			pg = art.Create(new ArticleModel
				{
					Language = new LanguageModel(language),
					Company = cm,
					Title = "Page With Xml",
					Content = ResourcesReader.LoadManifestResourceString("PageWithXml.txt", this),
					ParentId = homePage
				});
			tag.Create(pg, _tags[1]);

			pg = art.Create(new ArticleModel
				{
					Language = new LanguageModel(language),
					Company = cm,
					Title = "1 Page-Children",
					Content = "1 Page Content-With Children",
					ParentId = homePage
				});
			tag.Create(pg, _tags[2]);

			CreateChildren("1", art, pg, language, cm.Id, 4);
		}

		private void CreateChildren(string pageSource, IArticlesRepository art, long parentId, long languageId, long compId,
		                            int depth)
		{
			depth--;
			if (depth <= 0) return;
			var pg3 = art.Create(new ArticleModel
				{
					Language = new LanguageModel(languageId),
					Company = new CompanyModel(compId),
					Title = pageSource + ".1 Page",
					Content = pageSource + ".1 Page Content",
					ParentId = parentId
				});
			CreateChildren(pageSource + ".1", art, pg3, languageId, compId, depth);

			pg3 = art.Create(new ArticleModel
				{
					Language = new LanguageModel(languageId),
					Company = new CompanyModel(compId),
					Title = pageSource + ".2 Page",
					Content = pageSource + ".2 Page Content",
					ParentId = parentId
				});
			CreateChildren(pageSource + ".2", art, pg3, languageId, compId, depth);

			pg3 = art.Create(new ArticleModel
				{
					Language = new LanguageModel(languageId),
					Company = new CompanyModel(compId),
					Title = pageSource + ".3 Page",
					Content = pageSource + ".3 Page Content",
					ParentId = parentId
				});
			CreateChildren(pageSource + ".3", art, pg3, languageId, compId, depth);
		}
	}
}