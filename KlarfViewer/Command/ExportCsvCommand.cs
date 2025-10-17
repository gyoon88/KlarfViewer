
﻿using KlarfViewer.ViewModel;
﻿using System.Windows.Input;
﻿using KlarfViewer.Service;
﻿using Microsoft.Win32;
﻿using System.Linq;
﻿using System.Collections.Generic;
﻿
﻿namespace KlarfViewer.Command
﻿{
﻿    public class ExportCsvCommand
﻿    {
﻿        private readonly MainViewModel vm;
﻿        public ICommand ExportToCsvCommand { get; }
﻿
﻿        public ExportCsvCommand(MainViewModel MainVM)
﻿        {
﻿            vm = MainVM;
﻿            ExportToCsvCommand = new RelayCommand(ExecuteToCsv, CanExecuteToCsv);
﻿        }
﻿
﻿        private void ExecuteToCsv()
﻿        {
﻿            if (vm.DefectListVM.Defects == null || !vm.DefectListVM.Defects.Any())
﻿            {
﻿                return;
﻿            }
﻿
﻿            var sfd = new SaveFileDialog
﻿            {
﻿                Filter = "CSV File (*.csv)|*.csv",
﻿                FileName = $"{vm.DefectListVM.KlarfInfomation.Wafer.DeviceID}_Defects.csv"
﻿            };
﻿
﻿            if (sfd.ShowDialog() == true)
﻿            {
﻿                var headers = new string[] { "DEFECTID", "XINDEX", "YINDEX", "XSIZE", "YSIZE", "XREL", "YREL", "DEFECTAREA", "DSIZE", "DEFECTIDINDIE", "TOTALDEFECTSINDIE" };
﻿                var data = vm.DefectListVM.Defects.Select(d => new string[]
﻿                {
﻿                    d.Id.ToString(),

﻿                    d.XIndex.ToString(),
﻿                    d.YIndex.ToString(),

﻿                    d.XSize.ToString(),
﻿                    d.YSize.ToString(),

                    d.XRel.ToString(),
                    d.YRel.ToString(),

                    d.DefectArea.ToString(),
                    d.DSize.ToString(),
                    d.DefectIdInDie.ToString(),
                    d.TotalDefectsInDie.ToString(),
﻿                });
﻿
﻿                CsvExportService.Export(sfd.FileName, data, headers);
﻿            }
﻿        }
﻿        private bool CanExecuteToCsv()
﻿        {
﻿            return vm.DefectListVM.Defects.Any();
﻿        }
﻿    }
﻿}
﻿
