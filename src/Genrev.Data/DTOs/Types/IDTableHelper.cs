﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Genrev.Data.DTOs.Types
{
    class IDTableHelper
    {

        public static DataTable FromIntArray(int[] items) {

            var t = new DataTable();
            t.Columns.Add("ID");

            if (items != null) {

                foreach (var i in items) {
                    t.Rows.Add(i);
                }

            }
                 
            return t;
        }

    }
}
