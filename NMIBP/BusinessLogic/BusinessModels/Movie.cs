using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMIBP.BusinessLogic.BusinessModels
{
    public class Movie
    {
        private string _title;
        private string _categories;
        private string _summary;
        private string _description;

        public Movie()
        {
            _title = "";
            _categories = "";
            _summary = "";
            _description = "";
        }

        public Movie(string title, string categories, string summary, string description)
        {
            Title = title;
            Categories = categories;
            Summary = summary;
            Description = description;
        }

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException($"Invalid categories {value}");
                }
                _title = value;
            }
        }

        public string Categories
        {
            get
            {
                return _categories;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException($"Invalid title {value}");
                }
                _categories = value;
            }
        }

        public string Summary
        {
            get
            {
                return _summary;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException($"Invalid summary {value}");
                }
                _summary = value;
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException($"Invalid description {value}");
                }
                _description = value;
            }
        }
    }
}
