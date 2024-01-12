function onFileButtonClick() {
    // Define your document definition
    var docDefinition = {
        content: [
            'Hello, this is a sample PDF generated using pdfmake!'
        ]
    };

    // Generate PDF
    pdfMake.createPdf(docDefinition).open();
}

function downloadSingleInvoice(e) {


    console.log(e)

    //var docDefinition = {
    //    content: [
    //        'Hello, this is a sample PDF generated using pdfmake!'
    //    ]
    //};


    //pdfMake.createPdf(docDefinition).open();
}


$(document).ready(function () {
    $('.productButton').on('click', function () {
        
        var invoiceId = this.dataset.itemid;
        var docNumber = this.dataset.docnumber;
        var invoiceDate = this.dataset.invoicedate;
        var dueDate = this.dataset.duedate;
        var total = this.dataset.total;
        var paid = this.dataset.paid;
        var balance = this.dataset.balance;
        var status = this.dataset.status;


        var docDefinition = {
            pageMargins: [80, 80, 40, 60],
            header: getHeader(),
            content: [
                {
                    table: {
                        widths: ['45%', '45%'],
                        heights: [50,],
                        body: [
                            [{ text: 'Invoice Summary', margin: [0, 10, 0, 0], colSpan: 2, alignment: 'center', fillColor: 'slateblue', color: 'white', fontSize: '22' }, {}],
                            [{ text: 'Doc Number', color: 'gray', margin: [10, 10, 0, 5], fontSize: 16, border: [true, false, false, false] }, { text: 'Status', color: 'gray', margin: [0, 10, 0, 0], fontSize: 16, border: [false, false, true, false] }],
                            [{ text: docNumber, bold:true, margin: [10, 0, 0, 10], border: [true, false, false, false] }, { text: status, bold:true, border: [false, false, true, false] }],

                            [{ text: 'Invoice Date', color: 'gray', margin: [10, 0, 0, 5], fontSize: 16, border: [true, false, false, false] }, { text: 'Due Date', color: 'gray', fontSize: 16, border: [false, false, true, false] }],
                            [{ text: invoiceDate, bold:true, margin: [10, 0, 0, 10], border: [true, false, false, false] }, { text: dueDate, bold:true, border: [false, false, true, false] }],

                            [{ text: 'Total', color: 'gray', margin: [10, 0, 0, 5], fontSize: 16, border: [true, false, false, false] }, { text: 'Paid', color: 'gray', fontSize: 16, border: [false, false, true, false] }],
                            [{ text: total, bold:true, margin: [10, 0, 0, 10], border: [true, false, false, false] }, { text: paid, bold:true, border: [false, false, true, false] }],

                            [{ text: 'Balance', color: 'gray', margin: [10, 0, 0, 5], fontSize: 16, border: [true, false, false, false] }, { text: '', border: [false, false, true, false] }],
                            [{ text: balance, bold:true, margin: [10, 0, 0, 10], border: [true, false, false, true] }, { text: '', border: [false, false, true, true] }],

                        ]
                    }
                },
            ]
        };

        pdfMake.createPdf(docDefinition).open();
    });


    $('.statementPrintRowButton').on('click', function () {

        var transactionId = this.dataset.itemid;
        var refNumber = this.dataset.refno;
        var docNumber = this.dataset.docnumber;
        var createdDate = this.dataset.createdate;
        var transactionType = this.dataset.type;
        var debit = this.dataset.debit;
        var credit = this.dataset.credit;
        var balance = this.dataset.balance;


        var docDefinition = {
            pageMargins: [80, 80, 40, 60],
            header: getHeader(),
            content: [
                {
                    table: {
                        widths: ['45%', '45%'],
                        heights: [50,],
                        body: [
                            [{ text: 'Transaction Summary', margin: [0, 10, 0, 0], colSpan: 2, alignment: 'center', fillColor: 'slateblue', color: 'white', fontSize: '22' }, {}],
                            [{ text: 'Ref Number', color: 'gray', margin: [10, 10, 0, 5], fontSize: 16, border: [true, false, false, false] }, { text: 'Doc Number', color: 'gray', margin: [0, 10, 0, 0], fontSize: 16, border: [false, false, true, false] }],
                            [{ text: refNumber, bold: true, margin: [10, 0, 0, 10], border: [true, false, false, false] }, { text: docNumber, bold: true, border: [false, false, true, false] }],

                            [{ text: 'Transaction Date', color: 'gray', margin: [10, 0, 0, 5], fontSize: 16, border: [true, false, false, false] }, { text: 'Type', color: 'gray', fontSize: 16, border: [false, false, true, false] }],
                            [{ text: createdDate, bold: true, margin: [10, 0, 0, 10], border: [true, false, false, false] }, { text: transactionType, bold: true, border: [false, false, true, false] }],

                            [{ text: 'Debit', color: 'gray', margin: [10, 0, 0, 5], fontSize: 16, border: [true, false, false, false] }, { text: 'Credit', color: 'gray', fontSize: 16, border: [false, false, true, false] }],
                            [{ text: debit, bold: true, margin: [10, 0, 0, 10], border: [true, false, false, false] }, { text: credit, bold: true, border: [false, false, true, false] }],

                            [{ text: 'Balance', color: 'gray', margin: [10, 0, 0, 5], fontSize: 16, border: [true, false, false, false] }, { text: '', border: [false, false, true, false] }],
                            [{ text: balance, bold: true, margin: [10, 0, 0, 10], border: [true, false, false, true] }, { text: '', border: [false, false, true, true] }],

                        ]
                    }
                },
            ]
        };

        pdfMake.createPdf(docDefinition).open();
    });



    $('.paymentPrintRowButton').on('click', function () {

        var itemId = this.dataset.itemid
        var accountName = this.dataset.accountname.replace(/&nbsp;/g, " ")
        var receiverBank = this.dataset.receiverbank.replace(/&nbsp;/g, " ")
        var paymentMode = this.dataset.paymentmode
        var paymentDate = this.dataset.paymentdate.replace(/&nbsp;/g, " ")
        var paymentAmount = this.dataset.paymentamount

        var docDefinition = {
            pageMargins: [80, 80, 40, 60],
            header: getHeader(),
            content: [
                {
                    table: {
                        widths: ['45%', '45%'],
                        heights: [50,],
                        body: [
                            [{ text: 'Payment Summary', margin: [0, 10, 0, 0], colSpan: 2, alignment: 'center', fillColor: 'slateblue', color: 'white', fontSize: '22' }, {}],
                            [{ text: 'Account Name', color: 'gray', margin: [10, 10, 0, 5], fontSize: 16, border: [true, false, false, false] }, { text: 'Receiver Bank', color: 'gray', margin: [0, 10, 0, 0], fontSize: 16, border: [false, false, true, false] }],
                            [{ text: accountName, bold: true, margin: [10, 0, 0, 10], border: [true, false, false, false] }, { text: receiverBank, bold: true, border: [false, false, true, false] }],

                            [{ text: 'Payment Mode', color: 'gray', margin: [10, 0, 0, 5], fontSize: 16, border: [true, false, false, false] }, { text: 'Payment Date', color: 'gray', fontSize: 16, border: [false, false, true, false] }],
                            [{ text: paymentMode, bold: true, margin: [10, 0, 0, 10], border: [true, false, false, false] }, { text: paymentDate, bold: true, border: [false, false, true, false] }],

                            [{ text: 'Payment Amount', color: 'gray', margin: [10, 0, 0, 5], fontSize: 16, border: [true, false, false, false] }, { text: '', color: 'gray', fontSize: 16, border: [false, false, true, false] }],
                            [{ text: paymentAmount, bold: true, margin: [10, 0, 0, 10], border: [true, false, false, true] }, { text: '', bold: true, border: [false, false, true, true] }],
                        ]
                    }
                },
            ]
        };

        pdfMake.createPdf(docDefinition).open();
    });

});



function printPaymentSummary(paymentSummary) {
    console.log(paymentSummary)
    var docDefinition = {
        pageMargins: [80, 80, 40, 60],
        header: getHeader(),
        content: [
            {
                table: {
                    widths: ['45%', '45%'],
                    heights: [50,],
                    body: [
                        [{ text: 'Payment Summary', margin: [0, 10, 0, 0], colSpan: 2, alignment: 'center', fillColor: 'slateblue', color: 'white', fontSize: '22' }, {}],
                        [{ text: 'DATE OF TRANSACTION', color: 'gray', margin: [10, 10, 0, 5], fontSize: 16, border: [true, false, false, false] }, { text: 'TRANSACTION NUMBER', color: 'gray', margin: [0, 10, 0, 0], fontSize: 16, border: [false, false, true, false] }],
                        [{ text: paymentSummary.dateOfTransaction, bold: true, margin: [10, 0, 0, 10], border: [true, false, false, false] }, { text: paymentSummary.transactionNumber, bold: true, border: [false, false, true, false] }],

                        [{ text: 'NO. OF INVOICES', color: 'gray', margin: [10, 0, 0, 5], fontSize: 16, border: [true, false, false, false] }, { text: 'PAYMENT TYPE', color: 'gray', fontSize: 16, border: [false, false, true, false] }],
                        [{ text: paymentSummary.noOfInvoices, bold: true, margin: [10, 0, 0, 10], border: [true, false, false, false] }, { text: paymentSummary.paymentType, bold: true, border: [false, false, true, false] }],

                        [{ text: 'PAID TO', color: 'gray', margin: [10, 0, 0, 5], fontSize: 16, border: [true, false, false, false] }, { text: 'PAID VIA', color: 'gray', fontSize: 16, border: [false, false, true, false] }],
                        [{ text: paymentSummary.paidTo, bold: true, margin: [10, 0, 0, 10], border: [true, false, false, false] }, { text: paymentSummary.paidVia, bold: true, border: [false, false, true, false] }],

                        [{ text: 'MODE OF PAYMENT', color: 'gray', margin: [10, 0, 0, 5], fontSize: 16, border: [true, false, false, false] }, { text: 'BALANCE DUE', color:'gray', fontSize: 16, border: [false, false, true, false] }],
                        [{ text: paymentSummary.modeOfPayment, bold: true, margin: [10, 0, 0, 10], border: [true, false, false, true] }, { text: paymentSummary.balanceDue, border: [false, false, true, true] }],

                    ]
                }
            },
        ]
    };

    pdfMake.createPdf(docDefinition).open();

}




function getHeader() {
    return [{
        columns: [
            {
                image: smsaLogo,
                width: 60,
                height: 30,
            },
            //{
            //    text: "smsa®",
            //    margin: [5, 4, 0, 0],
            //    width: 420,
            //    fontSize: 12,
            //    color: "#707070",
            //},
            {
                text: 'Printed On: ' + new Date().toLocaleDateString("en-US"),
                margin: [0, 5, 0, 0],
                color: "#707070",
                alignment: 'right'
            }
        ],
        margin: [20, 20, 20, 20]
    }];
}

var smsaLogo = 'data:image/jpeg;base64,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'