using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.HabboHotel.Navigator
{
    public class SearchResultList
    {
        private int _id;
        private string _category;
        private string _categoryName;
        private string _customName;
        private bool _canDoActions;
        private int _colour;
        private int _requiredRank;
        private NavigatorViewMode _viewMode;
        private NavigatorCategoryType _categoryType;
        private NavigatorSearchAllowance _searchAllowance;
        private int _orderId;

        public SearchResultList(int Id, string Category, string CategoryIdentifier, string PublicName, bool CanDoActions, int Colour, int RequiredRank, NavigatorViewMode ViewMode, string CategoryType, string SearchAllowance, int OrderId)
        {
            this._id = Id;
            this._category = Category;
            this._categoryName = CategoryIdentifier;
            this._customName = PublicName;
            this._canDoActions = CanDoActions;
            this._colour = Colour;
            this._requiredRank = RequiredRank;
            this._viewMode = ViewMode;
            this._categoryType = NavigatorCategoryTypeUtility.GetCategoryTypeByString(CategoryType);
            this._searchAllowance = NavigatorSearchAllowanceUtility.GetSearchAllowanceByString(SearchAllowance);
            this._orderId = OrderId;
        }

        public int Id
        {
            get { return this._id; }
            set { this._id = value; }
        }

        //TODO: Make an enum?
        public string Category
        {
            get { return this._category; }
            set { this._category = value; }
        }

        public string CategoryIdentifier
        {
            get { return this._categoryName; }
            set { this._categoryName = value; }
        }

        public string PublicName
        {
            get { return this._customName; }
            set { this._customName = value; }
        }

        public bool CanDoActions
        {
            get { return this._canDoActions; }
            set { this._canDoActions = value; }
        }

        public int Colour
        {
            get { return this._colour; }
            set { this._colour = value; }
        }

        public int RequiredRank
        {
            get { return this._requiredRank; }
            set { this._requiredRank = value; }
        }

        public NavigatorViewMode ViewMode
        {
            get { return this._viewMode; }
            set { this._viewMode = value; }
        }

        public NavigatorCategoryType CategoryType
        {
            get { return this._categoryType; }
            set { this._categoryType = value; }
        }

        public NavigatorSearchAllowance SearchAllowance
        {
            get { return this._searchAllowance; }
            set { this._searchAllowance = value; }
        }

        public int OrderId
        {
            get { return this._orderId; }
            set { this._orderId = value; }
        }
    }
}
