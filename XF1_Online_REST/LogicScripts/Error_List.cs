using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XF1_Online_REST.LogicScripts
{
    public class Error_List
    {
        public List<string> errors { get; set; }

        private List<Boolean>conds;

        public Error_List(string error)
        {
            this.errors = new List<string>{error};
            this.conds = new List<Boolean> {false};

        }
        public Error_List()
        {
            this.errors = new List<string>();
            this.conds = new List<Boolean>();
        }
        public Error_List(List<string> errors,List<Boolean> conds)
        {
            this.errors = errors;
            this.conds = conds;
        }

        public void purgeErrorsList()
        {
            int ind = 0;
            List<string> newErrorList = new List<string>();
            List<Boolean> newCondList = new List<Boolean>();
            foreach (Boolean cond in this.conds)
            {
                if(!cond)
                {
                    newErrorList.Add(this.errors[ind]);
                    newCondList.Add(cond);
                }
                ind++;
            }
            this.errors=newErrorList;
            this.conds=newCondList;
        }
        public void addError(string error,Boolean cond)
        {
            this.errors.Add(error);
            this.conds.Add(cond);
        }
        public Boolean hasErrors()
        {
            foreach (Boolean cond in this.conds)
            {
                if(!cond)
                {
                    return true;
                }
            }
            return false;
        }
        public void fuse(Error_List error_List)
        {
            List<String> tempErrors = new List<string>(this.errors.Count + error_List.errors.Count);
            tempErrors.AddRange(this.errors);
            tempErrors.AddRange(error_List.errors);
            this.errors = tempErrors;

            List<Boolean> tempConds = new List<Boolean>(this.conds.Count + error_List.conds.Count);
            tempConds.AddRange(this.conds);
            tempConds.AddRange(error_List.conds);
            this.conds = tempConds;
        }
    }
}