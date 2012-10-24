using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;
using ZakCms.Models.Entitites;
using ZakCms.Plugins.AdditionalBehaviours;
using ZakCms.Plugins.Joins;
using ZakCms.Plugins.Selectors;
using ZakCms.Repositories;
using ZakDb.Plugins;
using ZakDb.Repositories;
using ZakDb.Repositories.Queries;
using ZakDb.Utils;
using ZakWeb.Renderers;
using ZakWeb.Utils.Commons;

namespace ZakCms.Factories
{
	public class SimpleFakeFactory
	{
		private readonly Dictionary<string, object> _singletons;
		private static SimpleFakeFactory _simpleFakeFactory;

		private SimpleFakeFactory()
		{
			_singletons = new Dictionary<string, object>();
		}


		public static void InitializeSimpleFakeFactory(bool backEnd = true, bool winForm = false, string path = null)
		{
			_simpleFakeFactory = new SimpleFakeFactory();
			_simpleFakeFactory.Initialize(backEnd, winForm, path);
		}

		// ReSharper disable RedundantAssignment
		private void Initialize(bool backEnd, bool winForm, string path)
		{
			object tmp;

			var dataDirectory = ConfigurationManager.AppSettings["DataDirectory"];

			if (!winForm)
			{
				path = HttpRuntime.AppDomainAppPath;
				var absoluteDataDirectory = Path.Combine(path, dataDirectory);
				var fullPAt = Path.GetFullPath(absoluteDataDirectory);
				AppDomain.CurrentDomain.SetData("DataDirectory", fullPAt);
			}

			string cmsDb = ConfigurationManager.ConnectionStrings["CmsDb"].ConnectionString;

			_singletons.Add("CmsDb", cmsDb);

			_singletons.Add("IRenderer", new MarkdownRenderer());

			_singletons.Add("ILanguagesRepository", new LanguagesRepository("Languages", cmsDb, new List<IRepositoryPlugin>()));

			_singletons.Add("ICompaniesRepository", new CompaniesRepository("Companies", cmsDb, new List<IRepositoryPlugin>()));

			_singletons.Add("ITagsRepository", new TagsRepository("Tags", cmsDb, new List<IRepositoryPlugin>()));

			_singletons.Add("IBEUsersRepository", new UsersRepository("BEUsers", cmsDb, new List<IRepositoryPlugin>()));

			_singletons.Add("IFEUsersRepository", new UsersRepository("FEUsers", cmsDb, new List<IRepositoryPlugin>()));

			string language = ConfigurationManager.AppSettings["CmsLanguage"];

			var lanRep = (ILanguagesRepository) Create("ILanguagesRepository");
			var allElsLan = lanRep.GetFirst(new QueryObject {WhereCondition = string.Format("Code='{0}'", language)});
			if (allElsLan == null) tmp = -1;
			else tmp = ((LanguageModel) allElsLan).Id;
			_singletons.Add("CmsLanguage", tmp);

			_singletons.Add("CmsLanguageString", language);


			string company = ConfigurationManager.AppSettings["CmsCompany"];
			var compRep = (ICompaniesRepository) Create("ICompaniesRepository");
			var allElsCmp = compRep.GetFirst(new QueryObject {WhereCondition = string.Format("Code='{0}'", company)});
			if (allElsCmp == null) tmp = -1;
			else tmp = ((CompanyModel) allElsCmp).Id;
			_singletons.Add("CmsCompany", tmp);

			if (!winForm)
			{
				_singletons.Add("IExternalParameters",
				                new SessionExternalParameters(
					                new Dictionary<string, object>
						                {
							                {"LanguageId", Create("CmsLanguage")},
							                {"IsAuthenticated", false},
							                {"CompanyId", Create("CmsCompany")}
						                }));

				_singletons.Add("ILanguageSelectorPlugin",
				                new NumericLanguageSelectorPlugin(
					                (IExternalParameters) Create("IExternalParameters"),
					                (long) Create("CmsLanguage"),
					                "LanguageId"));

				_singletons.Add("IArticleAuthenticationSelectorPlugin",
				                new ArticleAuthenticationSelectorPlugin(
					                (IExternalParameters) Create("IExternalParameters"),
					                "IsAuthenticated"));

				_singletons.Add("ICompanySelectorPlugin",
				                new CompanySelectorPlugin(
					                (IExternalParameters) Create("IExternalParameters"),
					                (long) Create("CmsCompany"),
					                "CompanyId"));
			}

			var feedsPlugs = new List<IRepositoryPlugin>
				{
					new ElementTimestampPlugin(),
					new JoinedLanguagePlugin((IRepository) Create("ILanguagesRepository")),
					new JoinedCompanyPlugin((IRepository) Create("ICompaniesRepository"))
				};
			if (!winForm)
			{
				feedsPlugs.Add((IRepositoryPlugin) Create("ICompanySelectorPlugin"));
			}
			_singletons.Add("IFeedsRepository", new FeedsRepository("Feeds", cmsDb, feedsPlugs));

			_singletons.Add("IFeedsContentRepository", new FeedsContentRepository("FeedsContent", cmsDb,
			                                                                      new List<IRepositoryPlugin>
				                                                                      {new ElementTimestampPlugin()}));

			_singletons.Add("IFeedsToTagsRepository", new FeedsToTagsRepository("FeedsToTags", cmsDb, new List<IRepositoryPlugin>
				{
					new JoinedTagPlugin((IRepository) Create("ITagsRepository"))
				}, (IFeedsRepository) Create("IFeedsRepository")));

			var articlePlugs = new List<IRepositoryPlugin>
				{
					new TreeRepositoryPlugin(),
					new ElementTimestampPlugin(),
					new JoinedLanguagePlugin((IRepository) Create("ILanguagesRepository")),
					new JoinedCompanyPlugin((IRepository) Create("ICompaniesRepository"))
				};
			if (!backEnd)
			{
				articlePlugs.Add((IRepositoryPlugin) Create("IArticleAuthenticationSelectorPlugin"));
			}
			if (!winForm)
			{
				articlePlugs.Add((IRepositoryPlugin) Create("ILanguageSelectorPlugin"));
				articlePlugs.Add((IRepositoryPlugin) Create("ICompanySelectorPlugin"));
			}

			_singletons.Add("IArticlesRepository", new ArticlesRepository("Articles", cmsDb, articlePlugs));

			_singletons.Add("IArticlesToTagsRepository",
			                new ArticlesToTagsRepository("ArticlesToTags", cmsDb, new List<IRepositoryPlugin>
				                {
					                new JoinedTagPlugin((IRepository) Create("ITagsRepository"))
				                }, (IArticlesRepository) Create("IArticlesRepository")));

			Create<IArticlesRepository>().AddPlugin(
				new ArticlesToFeedsPlugin(
					(IManyToManyRepository) Create("IArticlesToTagsRepository"),
					(IFeedsRepository) Create("IFeedsRepository"),
					(IFeedsContentRepository) Create("IFeedsContentRepository"),
					(IArticlesRepository) Create("IArticlesRepository"),
					(IFeedsToTagsRepository) Create("IFeedsToTagsRepository")
					));
			Create<IArticlesToTagsRepository>().AddPlugin(
				new ArticleTagsToFeedsPlugin(
					(IManyToManyRepository) Create("IArticlesToTagsRepository"),
					(IFeedsRepository) Create("IFeedsRepository"),
					(IFeedsContentRepository) Create("IFeedsContentRepository"),
					(IArticlesRepository) Create("IArticlesRepository"),
					(IFeedsToTagsRepository) Create("IFeedsToTagsRepository")
					));

			/*	((IArticlesRepository)Create<IArticlesRepository>()).AddPlugin(new ArticlesFeedsInteractionPlugin(
									(IFeedsRepository)Create("IFeedsRepository"),
									(IFeedsToTagsRepository)Create("IFeedsToTagsRepository"),
									(IFeedsContentRepository)Create("IFeedsContentRepository")));*/
		}

		// ReSharper restore RedundantAssignment
		public static object Create(string objectName)
		{
			switch (objectName)
			{
				case ("CmsSiteRoot"):
					{
						return ConfigurationManager.AppSettings["CmsSiteRoot"];
					}
				case ("CmsImagesRoot"):
					{
						return ConfigurationManager.AppSettings["CmsImagesRoot"];
					}
				default:
					{
						if (_simpleFakeFactory._singletons.ContainsKey(objectName))
						{
							return _simpleFakeFactory._singletons[objectName];
						}
					}
					throw new NotImplementedException("Element not imported by the factory: " + objectName);
			}
		}

		public static TItem Create<TItem>()
		{
			return (TItem) Create(typeof (TItem).Name);
		}
	}
}