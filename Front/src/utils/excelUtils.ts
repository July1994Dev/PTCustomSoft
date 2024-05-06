import Excel from 'exceljs';
import { saveAs } from 'file-saver';

export const exportToExcel = async (data: any[], fileName:string) => {
    const workbook = new Excel.Workbook();
    const sheet = workbook.addWorksheet('Datos');
    const headers = Object.keys(data[0]);
    const headerRow = sheet.addRow(headers);

    headerRow.eachCell((cell) => {
        cell.fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: 'FF1F4E78' },
            bgColor: { argb: 'FF1F4E78' }
        };
        cell.font = {
            color: { argb: 'FFFFFFFF' },
            bold: true
        };
    });

    data.forEach((item) => {
        sheet.addRow(Object.values(item));
    });

    sheet.columns.forEach((column: any) => {
        let maxLength = 0;
        column.eachCell({ includeEmpty: true }, (cell: any) => {
            let cellLength = cell.value ? cell.value.toString().length : 0;
            if (cellLength > maxLength) {
                maxLength = cellLength;
            }
        });
        column.width = maxLength < 10 ? 10 : maxLength;
    });

    const buffer = await workbook.xlsx.writeBuffer();
    const blob = new Blob([buffer], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
    saveAs(blob, fileName + '.xlsx');
};