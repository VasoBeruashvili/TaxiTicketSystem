function LogOutSafe() {
    window.localStorage.setItem('login', false);
    window.location = '/Account/Logout';
}

function IsNullOrEmpty(item) {
    return item !== '' && item !== null && item !== undefined && item !== 0;
}

function IsNullOrEmptyExceptZero(item) {
    return item !== '' && item !== null && item !== undefined;
}

function roundNumber(number, decimal_points) {
    if (!decimal_points) return Math.round(number);
    if (number == 0) {
        var decimals = "";
        for (var i = 0; i < decimal_points; i++) decimals += "0";
        return "0." + decimals;
    }

    var exponent = Math.pow(10, decimal_points);
    var num = Math.round((number * exponent)).toString();
    return num.slice(0, -1 * decimal_points) + "." + num.slice(-1 * decimal_points)
}

var app = angular.module('TaxiTicketSystemModule', ['ui.bootstrap', 'angularGrid']);

app.service('pickerService', function () {
    var date_from, date_to;

    var setDates = function (new_date_from, new_date_to) {
        date_from = new_date_from;
        date_to = new_date_to;
    }

    var getDates = function () {
        return { date_from: date_from, date_to: date_to };
    }

    var daysInMonth = function (m, y) {
        return 32 - moment(y, m, 32);
    }

    var getFromDateString = function () {
        return moment(date_from).format('YYYY-MM-DD') + ' 00:00:00';
    }

    var getToDateString = function () {
        return moment(date_to).format('YYYY-MM-DD') + ' 23:59:59';
    }

    return {
        setDates: setDates,
        getDates: getDates,
        daysInMonth: daysInMonth,
        getFromDateString: getFromDateString,
        getToDateString: getToDateString
    };
});

app.controller('TaxiTicketSystemController', function ($scope, $modal, $http, $filter, pickerService) {
    var dt = moment();

    $scope.openFromDatepicker = function ($event) {
        $scope.from_opened = true;
    };

    $scope.openToDatepicker = function ($event) {
        $scope.to_opened = true;
    };

    var filterDate = function (dat) {
        return dat.toDate();
    }

    $scope.date_from = filterDate(dt);
    $scope.date_to = filterDate(dt);

    $scope.$watch('date_from', function (newval, oldval) {
        pickerService.setDates($scope.date_from, $scope.date_to);
        if (newval !== oldval) {
        }
    }, true);

    $scope.$watch('date_to', function (newval, oldval) {
        pickerService.setDates($scope.date_from, $scope.date_to);
        if (newval !== oldval) {
        }
    }, true);

    $scope.menuChoiceClick = function (sign) {
        var year = dt.year();
        switch (sign) {
            case "today":
                $scope.date_from = filterDate(moment());
                $scope.date_to = filterDate(moment());
                break;
            case "year":
                $scope.date_from = filterDate(moment([year, 0, 1]));
                $scope.date_to = filterDate(moment([year, 11, 31]));
                break;
            case "kvartali1":
                $scope.date_from = filterDate(moment([year, 0, 1]));
                $scope.date_to = filterDate(moment([year, 2, 31]));
                break;
            case "kvartali2":
                $scope.date_from = filterDate(moment([year, 3, 1]));
                $scope.date_to = filterDate(moment([year, 5, 30]));
                break;
            case "kvartali3":
                $scope.date_from = filterDate(moment([year, 6, 1]));
                $scope.date_to = filterDate(moment([year, 8, 30]));
                break;
            case "kvartali4":
                $scope.date_from = filterDate(moment([year, 9, 1]));
                $scope.date_to = filterDate(moment([year, 11, 31]));
                break;
            case "yan":
                $scope.date_from = filterDate(moment([year, 0, 1]));
                $scope.date_to = filterDate(moment([year, 0, 31]));
                break;
            case "feb":
                $scope.date_from = filterDate(moment([year, 1, 1]));
                $scope.date_to = filterDate(moment([year, 1, moment([year, 1]).daysInMonth()]));
                break;
            case "mar":
                $scope.date_from = filterDate(moment([year, 2, 1]));
                $scope.date_to = filterDate(moment([year, 2, 31]));
                break;
            case "apr":
                $scope.date_from = filterDate(moment([year, 3, 1]));
                $scope.date_to = filterDate(moment([year, 3, 30]));
                break;
            case "may":
                $scope.date_from = filterDate(moment([year, 4, 1]));
                $scope.date_to = filterDate(moment([year, 4, 31]));
                break
            case "jun":
                $scope.date_from = filterDate(moment([year, 5, 1]));
                $scope.date_to = filterDate(moment([year, 5, 30]));
                break;
            case "jul":
                $scope.date_from = filterDate(moment([year, 6, 1]));
                $scope.date_to = filterDate(moment([year, 6, 31]));
                break;
            case "aug":
                $scope.date_from = filterDate(moment([year, 7, 1]));
                $scope.date_to = filterDate(moment([year, 7, 31]));
                break;
            case "sep":
                $scope.date_from = filterDate(moment([year, 8, 1]));
                $scope.date_to = filterDate(moment([year, 8, 30]));
                break;
            case "oct":
                $scope.date_from = filterDate(moment([year, 9, 1]));
                $scope.date_to = filterDate(moment([year, 9, 31]));
                break;
            case "nov":
                $scope.date_from = filterDate(moment([year, 10, 1]));
                $scope.date_to = filterDate(moment([year, 10, 30]));
                break;
            case "dec":
                $scope.date_from = filterDate(moment([year, 11, 1]));
                $scope.date_to = filterDate(moment([year, 11, 31]));
                break;
        }

        pickerService.setDates($scope.date_from, $scope.date_to);
    };

    $scope.OpenModal = function (title, rowData) {
        var modalInstance = $modal.open({
            animation: false,
            templateUrl: 'modal.html',
            controller: 'ModalController',
            size: 'lg',
            backdrop: true            
        });

        var date = moment().toDate();
        modalInstance.title = title;

        modalInstance.POid = rowData === undefined ? 0 : rowData.POid;
        modalInstance.PFid = rowData === undefined ? 0 : rowData.PFid;
        modalInstance.GDid = rowData === undefined ? 0 : rowData.GDid;        

        if (rowData === undefined) {
            modalInstance.dateNow = date;
        } else {
            modalInstance.dateNow = $filter('date')(rowData.dateNow, 'yyyy-MM-dd', '+0400');
        }

        modalInstance.vat = rowData === undefined ? '' : rowData.vat;
        modalInstance.contrVat = rowData === undefined ? '' : rowData.contrVat;
        modalInstance.amountPrice = rowData === undefined ? '' : rowData.amountPrice;
        modalInstance.contragentId = rowData === undefined ? '' : rowData.contragentId;
        modalInstance.staffID = rowData === undefined ? '' : rowData.staffID;
        modalInstance.startTime = rowData === undefined ? '' : rowData.startTime;
        modalInstance.endTime = rowData === undefined ? '' : rowData.endTime;
        modalInstance.traveledDistance = rowData === undefined ? '' : rowData.traveledDistance;
        modalInstance.parkingCosts = rowData === undefined ? '' : rowData.parkingCosts;
        modalInstance.customsFees = rowData === undefined ? '' : rowData.customsFees;
        modalInstance.additionalCosts = rowData === undefined ? '' : rowData.additionalCosts;
        modalInstance.withoutPrint = rowData === undefined ? '' : rowData.withoutPrint;
        modalInstance.commentOut = rowData === undefined ? '' : rowData.commentOut;
        //
        modalInstance.contragentFakeID = rowData === undefined ? '' : rowData.contragentFakeID;
        modalInstance.companyName = rowData === undefined ? '' : rowData.companyName;

        modalInstance.carFakeID = rowData === undefined ? '' : rowData.carFakeID;
        modalInstance.carNumber = rowData === undefined ? '' : rowData.carNumber;

        modalInstance.staffFakeID = rowData === undefined ? '' : rowData.staffFakeID;
        modalInstance.staffName = rowData === undefined ? '' : rowData.staffName;

        modalInstance.Deleted = rowData === undefined ? '' : rowData.Deleted;
    }

    var columnDefs = [
        { headerName: 'კომპანია', field: 'companyName', suppressSizeToFit: true, width: 250, headerTooltip: 'კომპანია' },
        { headerName: 'მგზავრობის თარიღი', field: 'dateNow', headerTooltip: 'მგზავრობის თარიღი' },
        {
            headerName: 'დაწყების დრო',
            field: 'startTime',
            cellRenderer: function (params) {
                if (params.value === null) {
                    return '';
                } else {
                    return '<p align="center">' + params.value + '</p>';
                }
            }, headerTooltip: 'დაწყების დრო'
        },
        {
            headerName: 'დასრულების დრო',
            field: 'endTime',
            cellRenderer: function (params) {
                if (params.value === null) {
                    return '';
                } else {
                    return '<p align="center">' + params.value + '</p>';
                }
            }, headerTooltip: 'დასრულების დრო'
        },
        { headerName: 'მძღოლი', field: 'staffName', suppressSizeToFit: true, width: 150, headerTooltip: 'მძღოლი' },
        {
            headerName: 'ავტომობილის N',
            field: 'carNumber',
            cellRenderer: function (params) {
                if (params.value === null) {
                    return '';
                } else {
                    return '<p align="center">' + params.value + '</p>';
                }
            }, headerTooltip: 'ავტომობილის N'
        },
        {
            headerName: 'გავლილი მანძილი',
            field: 'traveledDistance',
            filter: 'number',
            cellRenderer: function (params) {
                if (params.value === null) {
                    return '';
                } else {
                    return '<p align="right">' + params.value + '</p>';
                }
            }, headerTooltip: 'გავლილი მანძილი'
        },
        {
            headerName: 'ღირებულება',
            field: 'amountPrice',
            filter: 'number',
            cellRenderer: function (params) {
                if (params.value === null) {
                    return '';
                } else {
                    return '<p align="right">' + params.value + '</p>';
                }
            }, headerTooltip: 'ღირებულება'
        },
        {
            headerName: 'სადგომის ხარჯი',
            field: 'parkingCosts',
            filter: 'number',
            cellRenderer: function (params) {
                if (params.value === null) {
                    return '';
                } else {
                    return '<p align="right">' + params.value + '</p>';
                }
            }, headerTooltip: 'სადგომის ხარჯი'
        },
        {
            headerName: 'საბაჟოს მოსაკრებელი',
            field: 'customsFees',
            filter: 'number',
            cellRenderer: function (params) {
                if (params.value === null) {
                    return '';
                } else {
                    return '<p align="right">' + params.value + '</p>';
                }
            }, headerTooltip: 'საბაჟოს მოსაკრებელი'
        },
        {
            headerName: 'დამატებითი ხარჯი',
            field: 'additionalCosts',
            filter: 'number',
            cellRenderer: function (params) {
                if (params.value === null) {
                    return '';
                } else {
                    return '<p align="right">' + params.value + '</p>';
                }
            }, headerTooltip: 'დამატებითი ხარჯი'
        },
        {
            headerName: 'სულ ჯამი',
            field: 'TS',
            filter: 'number',
            cellRenderer: function (params) {
                if (params.value === null) {
                    return '';
                } else {
                    return '<p align="right">' + params.value + '</p>';
                }
            }, headerTooltip: 'სულ ჯამი'
        },
        {
            headerName: 'სულ ჯამი + დღგ',
            field: 'TSWV1',
            filter: 'number',
            cellRenderer: function (params) {
                if (params.value === null) {
                    return '';
                } else {
                    return '<p align="right">' + params.value + '</p>';
                }
            }, headerTooltip: 'სულ ჯამი + დღგ'
        },
        {
            headerName: 'სულ ჯამი - დღგ',
            field: 'TSWV2',
            filter: 'number',
            cellRenderer: function (params) {
                if (params.value === null) {
                    return '';
                } else {
                    return '<p align="right">' + params.value + '</p>';
                }
            }, headerTooltip: 'სულ ჯამი - დღგ'
        },
        { headerName: 'პრინტერის გარეშე', field: 'withoutPrint', headerTooltip: 'პრინტერის გარეშე' },
        { headerName: 'კომენტარი', field: 'commentOut', filter: 'text', headerTooltip: 'კომენტარი' }
    ]

    $scope.gridOptions = {
        columnDefs: columnDefs,
        rowData: [],
        enableColResize: true,
        rowSelection: 'single',
        enableFilter: true,
        rowClicked: function (row) {
            $scope.OpenModal('რედაქტირება', row.data);
        }      
        //pinnedColumnCount: 6,
        //enableSorting: true        
    };

    $scope.RefreshDocItems = function () {        
        app.GetDocItems();
    }

    $scope.getDeletedItems = false;

    $scope.init = function () {
        $scope.showChangePwd = false;
        $scope.isDisabled = 'disabled';

        window.addEventListener('resize', function (event) {
            $('#grdContIndex').css("height", $(window).height() - 115);
        });
        $('#grdContIndex').css("height", $(window).height() - 115);        

        $http.get('/Home/GetCurrentUser').then(function (response) {
            $scope.fullName = response.data.fullName;            
        });

        app.GetDocItems = function () {
            $scope.gridOptions.rowData = null;
            if ($scope.gridOptions.api) {
                $scope.gridOptions.api.onNewRows();
            }

            if ($scope.gridOptions.api !== undefined) {
                $scope.gridOptions.api.showLoading(true);
            }
            
            $http.post('/Home/GetDocItems', { fromDate: moment($scope.date_from).format('YYYY-MM-DD'), toDate: moment($scope.date_to).format('YYYY-MM-DD'), Deleted: $scope.getDeletedItems }).then(function (response) {
                angular.forEach(response.data.docItems, function (docItem) {
                    docItem.dateNow = moment(docItem.dateNow).format('YYYY-MM-DD');

                    docItem.parkingCosts = Math.round(parseFloat(docItem.parkingCosts === null || docItem.parkingCosts === '' ? '0' : docItem.parkingCosts.replace(",", ".")) * 100) / 100;
                    docItem.customsFees = Math.round(parseFloat(docItem.customsFees === null || docItem.customsFees === '' ? '0' : docItem.customsFees.replace(",", ".")) * 100) / 100;
                    docItem.additionalCosts = Math.round(parseFloat(docItem.additionalCosts === null || docItem.additionalCosts === '' ? '0' : docItem.additionalCosts.replace(",", ".")) * 100) / 100;
                    docItem.traveledDistance = Math.round(parseFloat(docItem.traveledDistance === null || docItem.traveledDistance === '' ? '0' : docItem.traveledDistance.replace(",", ".")) * 100) / 100;
                    
                    if (docItem.contrVat === '1') {
                        docItem.amountPrice = Math.round(docItem.amountPrice * 100) / 100;
                        docItem.TS = Math.round(docItem.amountPrice * 100) / 100;
                    }

                    if (docItem.contrVat === '2') {
                        docItem.TS = Math.round((docItem.amountPrice / 1.18) * 100) / 100;                        
                        docItem.TSWV1 = Math.round(docItem.amountPrice * 100) / 100;
                        docItem.amountPrice = Math.round((docItem.amountPrice / 1.18) * 100) / 100;
                    } else {
                        docItem.TSWV1 = '';
                    }

                    if (docItem.contrVat === '3') {
                        docItem.TS = roundNumber((docItem.amountPrice + (docItem.amountPrice * 0.18)), 1); //(docItem.amountPrice + (docItem.amountPrice * 0.18)).toFixed(1);
                        docItem.TSWV2 = Math.round(docItem.amountPrice * 100) / 100;
                        docItem.amountPrice = roundNumber((docItem.amountPrice + (docItem.amountPrice * 0.18)), 1); //(docItem.amountPrice + (docItem.amountPrice * 0.18)).toFixed(1);
                    } else {
                        docItem.TSWV2 = '';
                    }                    

                    docItem.amountPrice -= (docItem.parkingCosts + docItem.customsFees + docItem.additionalCosts);
                });
                $scope.gridOptions.rowData = response.data.docItems;
                if ($scope.gridOptions.api) {
                    $scope.gridOptions.api.onNewRows();
                }
                $scope.gridOptions.api.sizeColumnsToFit();
            });
        }

        //call this function
        //app.GetDocItems();
    }

    $scope.dateOptions = {
        startingDay: 1
    };
});

app.controller('ModalController', function ($scope, $modalInstance, $http) {
    $scope.init = function () {
        $scope.showSuccessAlert = false;
        $scope.showFailureAlert = false;

        $('#startTime').timeEntry({ show24Hours: true });
        $('#endTime').timeEntry({ show24Hours: true });        
    }

    $scope.title = $modalInstance.title;

    $scope.POid = $modalInstance.POid;
    $scope.PFid = $modalInstance.PFid;
    $scope.GDid = $modalInstance.GDid;

    $scope.dateNow = $modalInstance.dateNow;
    $scope.vat = $modalInstance.vat;
    $scope.contrVat = $modalInstance.contrVat;
    $scope.amountPrice = $modalInstance.amountPrice;
    $scope.contragentId = $modalInstance.contragentId;
    $scope.staffID = $modalInstance.staffID;
    $scope.startTime = $modalInstance.startTime;
    $scope.endTime = $modalInstance.endTime;
    $scope.traveledDistance = $modalInstance.traveledDistance;
    $scope.parkingCosts = $modalInstance.parkingCosts;
    $scope.customsFees = $modalInstance.customsFees;
    $scope.additionalCosts = $modalInstance.additionalCosts;
    $scope.withoutPrint = $modalInstance.withoutPrint === 'True' ? true : false;
    $scope.commentOut = $modalInstance.commentOut;
    //
    $scope.contragentFakeID = $modalInstance.contragentFakeID;
    $scope.companyName = $modalInstance.companyName;

    $scope.carFakeID = $modalInstance.carFakeID;
    $scope.carNumber = $modalInstance.carNumber;

    $scope.staffFakeID = $modalInstance.staffFakeID;
    $scope.staffName = $modalInstance.staffName;

    $scope.Deleted = $modalInstance.Deleted;

    $scope.GenerateModalSum = function () {
        $scope.modalSum = Math.round((parseFloat($scope.amountPrice || 0) + parseFloat($scope.parkingCosts || 0) + parseFloat($scope.customsFees || 0) + parseFloat($scope.additionalCosts || 0)) * 100) / 100;
        $scope.ValidateOtherRequiredInputs();
    }

    $scope.modalSum = Math.round((parseFloat($scope.amountPrice || 0) + parseFloat($scope.parkingCosts || 0) + parseFloat($scope.customsFees || 0) + parseFloat($scope.additionalCosts || 0)) * 100) / 100;

    if ($scope.title === 'დამატება') {
        $scope.showSaveAndNewButton = true;
        $scope.showDeleteButton = false;
    } else {
        $scope.showSaveAndNewButton = false;
        if ($scope.Deleted === 'false') {
            $scope.showDeleteButton = true;
            $scope.showRestoreButton = false;
        } else {
            $scope.showRestoreButton = true;
            $scope.showDeleteButton = false;
        }
    }

    $scope.validateInputs = function (txtCompanyName, cldDate, txtStaffName, txtCarName, txtTravDist, txtTPrice) {
        if (IsNullOrEmpty(txtCompanyName)) {
            document.getElementById('txtCompanyName').style.backgroundColor = "#fff";
        } else {
            document.getElementById('txtCompanyName').style.backgroundColor = "#ffb7b7";
        }

        if (IsNullOrEmpty(cldDate)) {
            document.getElementById('cldDate').style.backgroundColor = "#fff";
        } else {
            document.getElementById('cldDate').style.backgroundColor = "#ffb7b7";
        }

        if (IsNullOrEmpty(txtStaffName)) {
            document.getElementById('txtStaffName').style.backgroundColor = "#fff";
        } else {
            document.getElementById('txtStaffName').style.backgroundColor = "#ffb7b7";
        }

        if (IsNullOrEmpty(txtCarName)) {
            document.getElementById('txtCarName').style.backgroundColor = "#fff";
        } else {
            document.getElementById('txtCarName').style.backgroundColor = "#ffb7b7";
        }

        if (IsNullOrEmptyExceptZero(txtTravDist)) {
            document.getElementById('txtTravDist').style.backgroundColor = "#fff";
        } else {
            document.getElementById('txtTravDist').style.backgroundColor = "#ffb7b7";
        }

        if (IsNullOrEmptyExceptZero(txtTPrice)) {
            document.getElementById('txtTPrice').style.backgroundColor = "#fff";
        } else {
            document.getElementById('txtTPrice').style.backgroundColor = "#ffb7b7";
        }
    }

    $scope.validateModal = function () {
        var result = true;

        result = result && IsNullOrEmpty($scope.contragentId);
        $scope.validateInputs($scope.contragentId, $scope.dateNow, $scope.staffID, $scope.carNumber, $scope.traveledDistance, $scope.amountPrice);

        result = result && IsNullOrEmpty($scope.dateNow);
        $scope.validateInputs($scope.contragentId, $scope.dateNow, $scope.staffID, $scope.carNumber, $scope.traveledDistance, $scope.amountPrice);

        result = result && IsNullOrEmpty($scope.staffID);
        $scope.validateInputs($scope.contragentId, $scope.dateNow, $scope.staffID, $scope.carNumber, $scope.traveledDistance, $scope.amountPrice);

        result = result && IsNullOrEmpty($scope.carNumber);
        $scope.validateInputs($scope.contragentId, $scope.dateNow, $scope.staffID, $scope.carNumber, $scope.traveledDistance, $scope.amountPrice);

        result = result && IsNullOrEmptyExceptZero($scope.traveledDistance);
        $scope.validateInputs($scope.contragentId, $scope.dateNow, $scope.staffID, $scope.carNumber, $scope.traveledDistance, $scope.amountPrice);

        result = result && IsNullOrEmptyExceptZero($scope.amountPrice);
        $scope.validateInputs($scope.contragentId, $scope.dateNow, $scope.staffID, $scope.carNumber, $scope.traveledDistance, $scope.amountPrice);

        return result;
    }

    $scope.RestoreDefaults = function () {
        $scope.POid = 0;
        $scope.PFid = 0;
        $scope.GDid = 0;

        $scope.dateNow = new Date();
        $scope.amountPrice = '';
        $scope.contragentId = '';
        $scope.staffID = '';
        $scope.startTime = '';
        $scope.endTime = '';
        $scope.traveledDistance = '';
        $scope.parkingCosts = '';
        $scope.customsFees = '';
        $scope.additionalCosts = '';
        $scope.withoutPrint = '';
        $scope.commentOut = '';
        $scope.contragentFakeID = '';
        $scope.companyName = '';
        $scope.carFakeID = '';
        $scope.carNumber = '';
        $scope.staffFakeID = '';
        $scope.staffName = '';
        $scope.Deleted = 'false';
        $scope.modalSum = 0;
    }

    $scope.SaveAndNew = function () {
        if ($scope.validateModal()) {
            var docItem = {
                GeneralDocsItem: {
                    Id: $scope.GDid,
                    Tdate: $scope.dateNow,
                    Amount: parseFloat($scope.amountPrice),
                    ParamId1: $scope.contragentId,
                    TotalSumAll: $scope.modalSum,
                    customVat: $scope.vat                    
                },
                ProductOutItem: [
                    {
                        Id: $scope.POid,
                        StaffId: $scope.staffID,
                        StartTime: $scope.startTime,//
                        EndTime: $scope.endTime,//
                        TraveledDistance: $scope.traveledDistance,//
                        ParkingCosts: $scope.parkingCosts,//
                        CustomsFees: $scope.customsFees,//
                        AdditionalCosts: $scope.additionalCosts,//
                        WithoutPrint: $scope.withoutPrint,//
                        CommentOut: $scope.commentOut,//
                        CarNumber: $scope.carNumber,//
                        GeneralId: $scope.GDid,//
                        Deleted: $scope.Deleted,
                        contrVat: $scope.contrVat
                    }
                ],
                ProductsFlowList: [
                    {
                        Id: $scope.PFid,//
                        GeneralId: $scope.GDid//
                    }
                ]
            }

            $http.post("/Home/SaveDocItem", { docItem: docItem }).then(function (response) {
                if (response.data.saveResult) {                    
                    $scope.showSuccessAlert = true;
                    $scope.showFailureAlert = false;

                    setTimeout(function () {
                        $scope.showSuccessAlert = false;
                        $scope.showFailureAlert = false;

                        $scope.RestoreDefaults();

                        app.GetDocItems();
                    }, 1000);                    
                } else {
                    $scope.showSuccessAlert = false;
                    $scope.showFailureAlert = true;
                }
            }, function (response) {
                $scope.showSuccessAlert = false;
                $scope.showFailureAlert = true;
            });
        }
    }

    $scope.Save = function () {
        if ($scope.validateModal()) {
            var docItem = {
                GeneralDocsItem: {
                    Id: $scope.GDid,
                    Tdate: moment($scope.dateNow).format("YYYY-MM-DD"),
                    Amount: parseFloat($scope.amountPrice),
                    ParamId1: $scope.contragentId,
                    TotalSumAll: $scope.modalSum,
                    customVat: $scope.vat                    
                },
                ProductOutItem: [
                    {
                        Id: $scope.POid,
                        StaffId: $scope.staffID,
                        StartTime: $scope.startTime,//
                        EndTime: $scope.endTime,//
                        TraveledDistance: $scope.traveledDistance,//
                        ParkingCosts: $scope.parkingCosts,//
                        CustomsFees: $scope.customsFees,//
                        AdditionalCosts: $scope.additionalCosts,//
                        WithoutPrint: $scope.withoutPrint,//
                        CommentOut: $scope.commentOut,//
                        CarNumber: $scope.carNumber,//
                        GeneralId: $scope.GDid,//
                        Deleted: $scope.Deleted,
                        contrVat: $scope.contrVat
                    }
                ],
                ProductsFlowList: [
                    {
                        Id: $scope.PFid,//
                        GeneralId: $scope.GDid//
                    }
                ]
            }

            $http.post("/Home/SaveDocItem", { docItem: docItem }).then(function (response) {
                if (response.data.saveResult) {                    
                    $scope.showSuccessAlert = true;
                    $scope.showFailureAlert = false;

                    setTimeout(function () {
                        $scope.Cancel();

                        app.GetDocItems();
                    }, 1000);                    
                } else {
                    $scope.showSuccessAlert = false;
                    $scope.showFailureAlert = true;
                }
            }, function (response) {
                $scope.showSuccessAlert = false;
                $scope.showFailureAlert = true;
            });
        }
    };

    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');        
    };

    $scope.Delete = function () {
        $http.post('/Home/DeleteDocItem', { GeneralDocID: $scope.GDid }).then(function (response) {
            if (response.data.deleteResult) {
                $scope.Cancel();
                app.GetDocItems();
            } else {
                alert('ჩანაწერი ვერ წაიშალა!');
            }
        });
    }

    $scope.Restore = function () {
        $scope.Deleted = "false";
        $scope.Save();
    }

    $scope.GetContragentByFakeID = function () {
        $http.post('/Home/GetContragentByFakeID', { contragentFakeID: $scope.contragentFakeID }).then(function (response) {
            $scope.companyName = response.data.contragent.name;
            $scope.contragentId = response.data.contragent.id;
            $scope.vat = response.data.contragent.vat;
            $scope.contrVat = response.data.contragent.vat;

            $scope.validateInputs($scope.contragentId, $scope.dateNow, $scope.staffID, $scope.carNumber, $scope.traveledDistance, $scope.amountPrice);
        });
    }

    $scope.GetStaffByFakeID = function () {
        $http.post('/Home/GetStaffByFakeID', { staffFakeID: $scope.staffFakeID }).then(function (response) {
            $scope.staffName = response.data.staff.name;
            $scope.staffID = response.data.staff.id;

            $scope.validateInputs($scope.contragentId, $scope.dateNow, $scope.staffID, $scope.carNumber, $scope.traveledDistance, $scope.amountPrice);
        });
    }

    $scope.GetCarByFakeID = function () {
        $http.post('/Home/GetCarByFakeID', { carFakeID: $scope.carFakeID }).then(function (response) {
            $scope.carNumber = response.data.car.num;

            $scope.validateInputs($scope.contragentId, $scope.dateNow, $scope.staffID, $scope.carNumber, $scope.traveledDistance, $scope.amountPrice);
        });
    }

    $scope.ValidateOtherRequiredInputs = function () {
        $scope.validateInputs($scope.contragentId, $scope.dateNow, $scope.staffID, $scope.carNumber, $scope.traveledDistance, $scope.amountPrice);
    }

    $scope.openDatePicker = function ($event) {
        $scope.opened = true;
    };

    $scope.dateOptions = {
        startingDay: 1
    };
});

app.controller('ReportsController', function ($scope, $http, pickerService, $modal) {
    var dt = moment();

    $scope.openFromDatepicker = function ($event) {
        $scope.from_opened = true;
    };

    $scope.openToDatepicker = function ($event) {
        $scope.to_opened = true;
    };

    var filterDate = function (dat) {
        return dat.toDate();
    }

    $scope.date_from = filterDate(dt);
    $scope.date_to = filterDate(dt);

    $scope.$watch('date_from', function (newval, oldval) {
        pickerService.setDates($scope.date_from, $scope.date_to);
        if (newval !== oldval) {
        }
    }, true);

    $scope.$watch('date_to', function (newval, oldval) {
        pickerService.setDates($scope.date_from, $scope.date_to);
        if (newval !== oldval) {
        }
    }, true);

    $scope.menuChoiceClick = function (sign) {
        var year = dt.year();
        switch (sign) {
            case "today":
                $scope.date_from = filterDate(moment());
                $scope.date_to = filterDate(moment());
                break;
            case "year":
                $scope.date_from = filterDate(moment([year, 0, 1]));
                $scope.date_to = filterDate(moment([year, 11, 31]));
                break;
            case "kvartali1":
                $scope.date_from = filterDate(moment([year, 0, 1]));
                $scope.date_to = filterDate(moment([year, 2, 31]));
                break;
            case "kvartali2":
                $scope.date_from = filterDate(moment([year, 3, 1]));
                $scope.date_to = filterDate(moment([year, 5, 30]));
                break;
            case "kvartali3":
                $scope.date_from = filterDate(moment([year, 6, 1]));
                $scope.date_to = filterDate(moment([year, 8, 30]));
                break;
            case "kvartali4":
                $scope.date_from = filterDate(moment([year, 9, 1]));
                $scope.date_to = filterDate(moment([year, 11, 31]));
                break;
            case "yan":
                $scope.date_from = filterDate(moment([year, 0, 1]));
                $scope.date_to = filterDate(moment([year, 0, 31]));
                //$scope.XX ? $scope.monthNameOrDefault = '' : $scope.monthNameOrDefault = 'იანვარი';
                if (!$scope.XX) {
                    if ($scope.activeDetailedContragentReport === 'active') {
                        $scope.GetDetailedContragentReportsByContragentID();
                    }
                    if ($scope.activeBalanceReport === 'active') {
                        $scope.GetBalanceReportsByContragentID();
                    }
                }
                break;
            case "feb":
                $scope.date_from = filterDate(moment([year, 1, 1]));
                $scope.date_to = filterDate(moment([year, 1, moment([year, 1]).daysInMonth()]));
                //$scope.XX ? $scope.monthNameOrDefault = '' : $scope.monthNameOrDefault = 'თებერვალი';
                if (!$scope.XX) {
                    if ($scope.activeDetailedContragentReport === 'active') {
                        $scope.GetDetailedContragentReportsByContragentID();
                    }
                    if ($scope.activeBalanceReport === 'active') {
                        $scope.GetBalanceReportsByContragentID();
                    }
                }
                break;
            case "mar":
                $scope.date_from = filterDate(moment([year, 2, 1]));
                $scope.date_to = filterDate(moment([year, 2, 31]));
                //$scope.XX ? $scope.monthNameOrDefault = '' : $scope.monthNameOrDefault = 'მარტი';
                if (!$scope.XX) {
                    if ($scope.activeDetailedContragentReport === 'active') {
                        $scope.GetDetailedContragentReportsByContragentID();
                    }
                    if ($scope.activeBalanceReport === 'active') {
                        $scope.GetBalanceReportsByContragentID();
                    }
                }
                break;
            case "apr":
                $scope.date_from = filterDate(moment([year, 3, 1]));
                $scope.date_to = filterDate(moment([year, 3, 30]));
                //$scope.XX ? $scope.monthNameOrDefault = '' : $scope.monthNameOrDefault = 'აპრილი';
                if (!$scope.XX) {
                    if ($scope.activeDetailedContragentReport === 'active') {
                        $scope.GetDetailedContragentReportsByContragentID();
                    }
                    if ($scope.activeBalanceReport === 'active') {
                        $scope.GetBalanceReportsByContragentID();
                    }
                }
                break;
            case "may":
                $scope.date_from = filterDate(moment([year, 4, 1]));
                $scope.date_to = filterDate(moment([year, 4, 31]));
                //$scope.XX ? $scope.monthNameOrDefault = '' : $scope.monthNameOrDefault = 'მაისი';
                if (!$scope.XX) {
                    if ($scope.activeDetailedContragentReport === 'active') {
                        $scope.GetDetailedContragentReportsByContragentID();
                    }
                    if ($scope.activeBalanceReport === 'active') {
                        $scope.GetBalanceReportsByContragentID();
                    }
                }
                break
            case "jun":
                $scope.date_from = filterDate(moment([year, 5, 1]));
                $scope.date_to = filterDate(moment([year, 5, 30]));
                //$scope.XX ? $scope.monthNameOrDefault = '' : $scope.monthNameOrDefault = 'ივნისი';
                if (!$scope.XX) {
                    if ($scope.activeDetailedContragentReport === 'active') {
                        $scope.GetDetailedContragentReportsByContragentID();
                    }
                    if ($scope.activeBalanceReport === 'active') {
                        $scope.GetBalanceReportsByContragentID();
                    }
                }
                break;
            case "jul":
                $scope.date_from = filterDate(moment([year, 6, 1]));
                $scope.date_to = filterDate(moment([year, 6, 31]));
                //$scope.XX ? $scope.monthNameOrDefault = '' : $scope.monthNameOrDefault = 'ივლისი';
                if (!$scope.XX) {
                    if ($scope.activeDetailedContragentReport === 'active') {
                        $scope.GetDetailedContragentReportsByContragentID();
                    }
                    if ($scope.activeBalanceReport === 'active') {
                        $scope.GetBalanceReportsByContragentID();
                    }
                }
                break;
            case "aug":
                $scope.date_from = filterDate(moment([year, 7, 1]));
                $scope.date_to = filterDate(moment([year, 7, 31]));
                //$scope.XX ? $scope.monthNameOrDefault = '' : $scope.monthNameOrDefault = 'აგვისტო';
                if (!$scope.XX) {
                    if ($scope.activeDetailedContragentReport === 'active') {
                        $scope.GetDetailedContragentReportsByContragentID();
                    }
                    if ($scope.activeBalanceReport === 'active') {
                        $scope.GetBalanceReportsByContragentID();
                    }
                }
                break;
            case "sep":
                $scope.date_from = filterDate(moment([year, 8, 1]));
                $scope.date_to = filterDate(moment([year, 8, 30]));
                //$scope.XX ? $scope.monthNameOrDefault = '' : $scope.monthNameOrDefault = 'სექტემბერი';
                if (!$scope.XX) {
                    if ($scope.activeDetailedContragentReport === 'active') {
                        $scope.GetDetailedContragentReportsByContragentID();
                    }
                    if ($scope.activeBalanceReport === 'active') {
                        $scope.GetBalanceReportsByContragentID();
                    }
                }
                break;
            case "oct":
                $scope.date_from = filterDate(moment([year, 9, 1]));
                $scope.date_to = filterDate(moment([year, 9, 31]));
                //$scope.XX ? $scope.monthNameOrDefault = '' : $scope.monthNameOrDefault = 'ოქტომბერი';
                if (!$scope.XX) {
                    if ($scope.activeDetailedContragentReport === 'active') {
                        $scope.GetDetailedContragentReportsByContragentID();
                    }
                    if ($scope.activeBalanceReport === 'active') {
                        $scope.GetBalanceReportsByContragentID();
                    }
                }
                break;
            case "nov":
                $scope.date_from = filterDate(moment([year, 10, 1]));
                $scope.date_to = filterDate(moment([year, 10, 30]));
                //$scope.XX ? $scope.monthNameOrDefault = '' : $scope.monthNameOrDefault = 'ნოემბერი';
                if (!$scope.XX) {
                    if ($scope.activeDetailedContragentReport === 'active') {
                        $scope.GetDetailedContragentReportsByContragentID();
                    }
                    if ($scope.activeBalanceReport === 'active') {
                        $scope.GetBalanceReportsByContragentID();
                    }
                }
                break;
            case "dec":
                $scope.date_from = filterDate(moment([year, 11, 1]));
                $scope.date_to = filterDate(moment([year, 11, 31]));
                //$scope.XX ? $scope.monthNameOrDefault = '' : $scope.monthNameOrDefault = 'დეკემბერი';
                if (!$scope.XX) {
                    if ($scope.activeDetailedContragentReport === 'active') {
                        $scope.GetDetailedContragentReportsByContragentID();
                    }
                    if ($scope.activeBalanceReport === 'active') {
                        $scope.GetBalanceReportsByContragentID();
                    }
                }
                break;
        }

        pickerService.setDates($scope.date_from, $scope.date_to);
    };

    $scope.columnDefs = [
        { headerName: 'კონტრაგენტი', field: 'companyName', width: 230, suppressSizeToFit: true, headerTooltip: 'კონტრაგენტი' },
        { headerName: 'მგზავრობის თარიღი', field: 'dateNow', width: 170, suppressSizeToFit: true, headerTooltip: 'მგზავრობის თარიღი' },
        { headerName: 'მძღოლი', field: 'staffName', width: 150, suppressSizeToFit: true, headerTooltip: 'მძღოლი' },
        {
            headerName: 'ავტომობილის N',
            field: 'carNumber',
            cellRenderer: function (params) {
                if (params.value === null) {
                    return '';
                } else {
                    return '<p align="center">' + params.value + '</p>';
                }
            }, headerTooltip: 'ავტომობილის N'
        },
        {
            headerName: 'გავლილი მანძილი',
            field: 'traveledDistance',
            filter: 'number',
            cellRenderer: function (params) {
                if (params.value === null) {
                    return '';
                } else {
                    return '<p align="right">' + params.value + '</p>';
                }
            }, headerTooltip: 'გავლილი მანძილი'
        },
        {
            headerName: 'ღირებულება',
            field: 'amountPrice',
            filter: 'number',
            cellRenderer: function (params) {
                if (params.value === null) {
                    return '';
                } else {
                    return '<p align="right">' + params.value + '</p>';
                }
            }, headerTooltip: 'ღირებულება'
        },
        {
            headerName: 'სადგომის ხარჯი',
            field: 'parkingCosts',
            filter: 'number',
            cellRenderer: function (params) {
                if (params.value === null) {
                    return '';
                } else {
                    return '<p align="right">' + params.value + '</p>';
                }
            }, headerTooltip: 'სადგომის ხარჯი'
        },
        {
            headerName: 'საბაჟოს მოსაკრებელი',
            field: 'customsFees',
            filter: 'number',
            cellRenderer: function (params) {
                if (params.value === null) {
                    return '';
                } else {
                    return '<p align="right">' + params.value + '</p>';
                }
            }, headerTooltip: 'საბაჟოს მოსაკრებელი'
        },
        {
            headerName: 'დამატებითი ხარჯი',
            field: 'additionalCosts',
            filter: 'number',
            cellRenderer: function (params) {
                if (params.value === null) {
                    return '';
                } else {
                    return '<p align="right">' + params.value + '</p>';
                }
            }, headerTooltip: 'დამატებითი ხარჯი'
        },
        {
            headerName: 'სულ ჯამი',
            field: 'subTotalSum',
            filter: 'number',
            cellRenderer: function (params) {
                if (params.value === null) {
                    return '';
                } else {
                    return '<p align="right">' + params.value + '</p>';
                }
            }, headerTooltip: 'სულ ჯამი'
        },
        {
            headerName: 'სულ ჯამი + დღგ',
            field: 'TSWV1',
            filter: 'number',
            cellRenderer: function (params) {
                if (params.value === null) {
                    return '';
                } else {
                    return '<p align="right">' + params.value + '</p>';
                }
            }, headerTooltip: 'სულ ჯამი + დღგ'
        },
        {
            headerName: 'სულ ჯამი - დღგ',
            field: 'TSWV2',
            filter: 'number',
            cellRenderer: function (params) {
                if (params.value === null) {
                    return '';
                } else {
                    return '<p align="right">' + params.value + '</p>';
                }
            }, headerTooltip: 'სულ ჯამი - დღგ'
        },
        { headerName: 'პრინტერის გარეშე', field: 'withoutPrint', width: 80, suppressSizeToFit: true, headerTooltip: 'პრინტერის გარეშე' },
        { headerName: 'კომენტარი', field: 'commentOut', filter: 'text', headerTooltip: 'კომენტარი' }
    ]

    $scope.gridOptions = {
        columnDefs: $scope.columnDefs,
        rowData: [],
        enableColResize: true,
        rowSelection: 'single',
        enableFilter: true,
        rowClicked: function (row) {
            if ($scope.activeBalanceReport === 'active') {
                $scope.OpenSubReportsModal(row.data.id, row.data.dateFrom, row.data.dateTo);
            }
        }
    };

    $scope.EnumReportTypes = {
        DetailedContragentReport: 1,
        BalanceReport: 2,
        DetailedDriverReport: 3
    }

    $scope.GetReportByType = function (reportType) {
        switch (reportType) {
            case $scope.EnumReportTypes.DetailedContragentReport: {
                $('#grdContReports').css("height", $(window).height() - 115);

                $scope.activeBalanceReport = '';
                $scope.activeDetailedDriverReport = '';
                $scope.activeDetailedContragentReport = 'active';
                $scope.showTopPre = false;
                $scope.showTotalSum = true;
                $scope.showBottomPre = false;
                $scope.showPeriod = true;
                $scope.showYearMonth = false;

                $scope.showBtnRefresh = true;

                $scope.showCmbContragents = true && $scope.XX;
                $scope.showCmbDrivers = false;
                $scope.showWithCreateDate = false;

                $scope.startBalance = '';
                $scope.endBalance = '';
                $scope.incomeSum = '';
                $scope.outcomeSum = '';

                $scope.selectedContragent = undefined;
                $scope.date_from = new Date();
                $scope.date_to = new Date();
                $scope.date_from = filterDate(dt);
                $scope.date_to = filterDate(dt);
                //$scope.XX ? $scope.btnMonthStyle = '' : $scope.btnMonthStyle = 'margin-left: 0;';
                //$scope.XX ? $scope.monthNameOrDefault = '' : $scope.monthNameOrDefault = 'აირჩიეთ თვე';

                $scope.gridOptions.columnDefs = $scope.columnDefs;
                $scope.gridOptions.api.onNewCols();
                $scope.gridOptions.rowData = [];
                $scope.gridOptions.api.onNewRows();                

                break;
            }
            case $scope.EnumReportTypes.BalanceReport: {
                $('#grdContReports').css("height", $(window).height() - 145);

                $scope.activeDetailedContragentReport = '';
                $scope.activeDetailedDriverReport = '';
                $scope.activeBalanceReport = 'active';
                $scope.showTotalSum = false;
                $scope.showBottomPre = true;
                $scope.showPeriod = false;
                $scope.showYearMonth = true;

                $scope.showBtnRefresh = $scope.XX;

                $scope.showCmbContragents = true && $scope.XX;
                $scope.showCmbDrivers = false;
                $scope.showWithCreateDate = false;

                $scope.totalSum = '';

                $scope.selectedContragent = undefined;
                $scope.date_from = new Date();
                $scope.date_to = new Date();
                $scope.date_from = filterDate(dt);
                $scope.date_to = filterDate(dt);

                $scope.gridOptions.columnDefs = [
                    {
                        headerName: 'ID',
                        field: 'fakeID',
                        width: 50,
                        suppressSizeToFit: true, headerTooltip: 'ID'
                    },
                    {
                        headerName: 'კომპანია',
                        field: 'companyName',
                        width: 500, headerTooltip: 'კომპანია'
                    },
                    {
                        headerName: 'წინა პერიოდი',
                        field: 'prevPeriod',
                        filter: 'number',
                        width: 150,
                        cellRenderer: function (params) {
                            if (params.value === null) {
                                return '';
                            } else {
                                return '<p align="right">' + params.value + '</p>';
                            }
                        }, headerTooltip: 'წინა პერიოდი'
                    },
                    {
                        headerName: 'მიმდინარე',
                        field: 'currentPeriod',
                        filter: 'number',
                        width: 150,
                        cellRenderer: function (params) {
                            if (params.value === null) {
                                return '';
                            } else {
                                return '<p align="right">' + params.value + '</p>';
                            }
                        }, headerTooltip: 'მიმდინარე'
                    },
                    {
                        headerName: 'ჯამი',
                        field: 'totalPeriod',
                        filter: 'number',
                        width: 150,
                        cellRenderer: function (params) {
                            if (params.value === null) {
                                return '';
                            } else {
                                return '<p align="right">' + params.value + '</p>';
                            }
                        }, headerTooltip: 'ჯამი'
                    },
                ];
                $scope.gridOptions.api.onNewCols();                

                $scope.gridOptions.rowData = [];
                $scope.gridOptions.api.onNewRows();

                $scope.openYearMonthpicker = function ($event) {
                    $scope.yearmonth_opened = true;
                };

                $scope.year = filterDate(dt);

                $scope.$watch('year', function (newval, oldval) {
                    $scope.GetBalanceReportsByContragentID();
                }, true);

                break;
            }
            case $scope.EnumReportTypes.DetailedDriverReport: {
                $('#grdContReports').css("height", $(window).height() - 115);

                $scope.activeDetailedContragentReport = '';
                $scope.activeBalanceReport = '';
                $scope.activeDetailedDriverReport = 'active';
                $scope.showTopPre = false;
                $scope.showTotalSum = true;
                $scope.showBottomPre = false;
                $scope.showPeriod = true;
                $scope.showYearMonth = false;

                $scope.showBtnRefresh = $scope.XX;

                $scope.showCmbContragents = false;
                $scope.showCmbDrivers = true;
                $scope.showWithCreateDate = true;

                $scope.totalSum = '';

                $scope.selectedDriver = undefined;
                $scope.date_from = new Date();
                $scope.date_to = new Date();
                $scope.date_from = filterDate(dt);
                $scope.date_to = filterDate(dt);

                $scope.gridOptions.columnDefs = [
                    { headerName: 'კომპანიის ID', field: 'contragentFakeID', width: 80, suppressSizeToFit: true, headerTooltip: 'კომპანიის ID' },
                    { headerName: 'მძღოლი', field: 'staffName', width: 230, suppressSizeToFit: true, headerTooltip: 'მძღოლი' },
                    { headerName: 'მგზავრობის თარიღი', field: 'dateNow', width: 170, suppressSizeToFit: true, headerTooltip: 'მგზავრობის თარიღი' },
                    { headerName: 'კონტრაგენტი', field: 'contragentName', width: 230, suppressSizeToFit: true, headerTooltip: 'კონტრაგენტი' },
                    {
                        headerName: 'ავტომობილის N',
                        field: 'carNumber',
                        cellRenderer: function (params) {
                            if (params.value === null) {
                                return '';
                            } else {
                                return '<p align="center">' + params.value + '</p>';
                            }
                        }, headerTooltip: 'ავტომობილის N'
                    },
                    {
                        headerName: 'გავლილი მანძილი',
                        field: 'traveledDistance',
                        filter: 'number',
                        width: 170,
                        cellRenderer: function (params) {
                            if (params.value === null) {
                                return '';
                            } else {
                                return '<p align="right">' + params.value + '</p>';
                            }
                        }, headerTooltip: 'გავლილი მანძილი'
                    },
                    {
                        headerName: 'ღირებულება',
                        field: 'amountPrice',
                        filter: 'number',
                        width: 150,
                        cellRenderer: function (params) {
                            if (params.value === null) {
                                return '';
                            } else {
                                return '<p align="right">' + params.value + '</p>';
                            }
                        }, headerTooltip: 'ღირებულება'
                    },
                    {
                        headerName: 'სადგომის ხარჯი',
                        field: 'parkingCosts',
                        filter: 'number',
                        width: 160,
                        cellRenderer: function (params) {
                            if (params.value === null) {
                                return '';
                            } else {
                                return '<p align="right">' + params.value + '</p>';
                            }
                        }, headerTooltip: 'სადგომის ხარჯი'
                    },
                    {
                        headerName: 'საბაჟოს მოსაკრებელი',
                        field: 'customsFees',
                        filter: 'number',
                        cellRenderer: function (params) {
                            if (params.value === null) {
                                return '';
                            } else {
                                return '<p align="right">' + params.value + '</p>';
                            }
                        }, headerTooltip: 'საბაჟოს მოსაკრებელი'
                    },
                    {
                        headerName: 'დამატებითი ხარჯი',
                        field: 'additionalCosts',
                        filter: 'number',
                        cellRenderer: function (params) {
                            if (params.value === null) {
                                return '';
                            } else {
                                return '<p align="right">' + params.value + '</p>';
                            }
                        }, headerTooltip: 'დამატებითი ხარჯი'
                    },
                    {
                        headerName: 'სულ ჯამი',
                        field: 'subTotalSum',
                        filter: 'number',
                        cellRenderer: function (params) {
                            if (params.value === null) {
                                return '';
                            } else {
                                return '<p align="right">' + params.value + '</p>';
                            }
                        }, headerTooltip: 'სულ ჯამი'
                    },
                    {
                        headerName: 'სულ ჯამი + დღგ',
                        field: 'TSWV1',
                        filter: 'number',
                        cellRenderer: function (params) {
                            if (params.value === null) {
                                return '';
                            } else {
                                return '<p align="right">' + params.value + '</p>';
                            }
                        }, headerTooltip: 'სულ ჯამი + დღგ'
                    },
                    {
                        headerName: 'სულ ჯამი - დღგ',
                        field: 'TSWV2',
                        filter: 'number',
                        cellRenderer: function (params) {
                            if (params.value === null) {
                                return '';
                            } else {
                                return '<p align="right">' + params.value + '</p>';
                            }
                        }, headerTooltip: 'სულ ჯამი - დღგ'
                    },
                    { headerName: 'პრინტერის გარეშე', field: 'withoutPrint', width: 80, suppressSizeToFit: true, headerTooltip: 'პრინტერის გარეშე' },
                    { headerName: 'კომენტარი', field: 'commentOut', filter: 'text', headerTooltip: 'კომენტარი' }
                ];
                $scope.gridOptions.api.onNewCols();

                $scope.gridOptions.rowData = [];
                $scope.gridOptions.api.onNewRows();

                break;
            }
        }
    }

    $scope.init = function () {
        $http.get('/Home/GeneratePage').then(function (response) {
            $scope.XX = response.data.x.x;
            $scope.showDetailedDriverReport = $scope.XX;
            $scope.showHomeMenu = $scope.XX;
            $scope.showContragentsMenu = $scope.XX;
            $scope.showDebitAccountsMenu = $scope.XX;
            $scope.showPeriod = true;
            $scope.showYearMonth = false;
            //$scope.XX ? $scope.monthNameOrDefault = '' : $scope.monthNameOrDefault = 'აირჩიეთ თვე';

            $scope.showChangePwd = !$scope.XX;
            $scope.isDisabled = $scope.showChangePwd ? '' : 'disabled';

            $scope.showCmbContragents = true && $scope.XX;

            $scope.showFromDateCal = true;
            $scope.showToDateCal = true;
            $scope.showPeriodSpan = true;
            //$scope.XX ? $scope.btnMonthStyle = '' : $scope.btnMonthStyle = 'margin-left: 0;'
            //$scope.XX ? $scope.monthNameOrDefault = '' : $scope.monthNameOrDefault = 'აირჩიეთ თვე';
            $scope.showBtnRefresh = true;
            $scope.XX ? $scope.showMonthItem = true : $scope.showMonthItem = false;
        });        

        window.addEventListener('resize', function (event) {
            $scope.activeBalanceReport === 'active' ? $('#grdContReports').css("height", $(window).height() - 145) : $('#grdContReports').css("height", $(window).height() - 115);
        });
        $('#grdContReports').css("height", $(window).height() - 115);

        $http.get('/Home/GetCurrentUser').then(function (response) {
            $scope.fullName = response.data.fullName;
            $scope.contrID = response.data.contragentID;
        });

        $scope.activeDetailedContragentReport = 'active';

        $http.get('/Home/GetContragents').then(function (response) {
            angular.forEach(response.data.contragents, function (c) {
                c.name = c.fakeID + ' - ' + c.name;
            });
            $scope.contragents = response.data.contragents;
        });

        $http.get('/Home/GetDrivers').then(function (response) {
            angular.forEach(response.data.drivers, function (d) {
                d.name = d.fakeID + ' - ' + d.name;
            });
            $scope.drivers = response.data.drivers;
        });

        $scope.showTotalSum = true;
        $scope.showWithCreateDate = false;
        $scope.withCreateDate = false;
    }

    $scope.GetDetailedContragentReportsByContragentID = function () {
        //if ($scope.selectedContragent !== undefined && $scope.XX) {
            $scope.gridOptions.api.showLoading(true);
            $http.post('/Home/GetDetailedContragentReportsByContragentID', { contragentID: $scope.contrID === null ? $scope.selectedContragent === undefined ? null : $scope.selectedContragent.id : $scope.contrID, fromDate: moment($scope.date_from).format('YYYY-MM-DD'), toDate: moment($scope.date_to).format('YYYY-MM-DD') }).then(function (response) {
                angular.forEach(response.data.reports, function (report) {
                    report.dateNow = moment(report.dateNow).format('YYYY-MM-DD') + ' ' + report.startTime + '-' + report.endTime;
                    //report.amountPrice = Math.round((report.amountPrice / 1.18) * 100) / 100;
                    report.parkingCosts = Math.round(report.parkingCosts * 100) / 100;
                    report.customsFees = Math.round(report.customsFees * 100) / 100;
                    report.additionalCosts = Math.round(report.additionalCosts * 100) / 100;
                    
                    if (report.contrVat === '1') {
                        report.amountPrice = Math.round(report.amountPrice * 100) / 100;
                        report.TS = Math.round(report.amountPrice * 100) / 100;
                    }

                    if (report.contrVat === '2') {
                        report.TS = Math.round((report.amountPrice / 1.18) * 100) / 100;
                        report.TSWV1 = Math.round(report.amountPrice * 100) / 100;
                        report.amountPrice = Math.round((report.amountPrice / 1.18) * 100) / 100;
                    } else {
                        report.TSWV1 = '';
                    }

                    if (report.contrVat === '3') {
                        report.TS = roundNumber((report.amountPrice + (report.amountPrice * 0.18)), 1); //(report.amountPrice + (report.amountPrice * 0.18)).toFixed(2);
                        report.TSWV2 = Math.round(report.amountPrice * 100) / 100;
                        report.amountPrice = roundNumber((report.amountPrice + (report.amountPrice * 0.18)), 1); //(report.amountPrice + (report.amountPrice * 0.18)).toFixed(2);
                    } else {
                        report.TSWV2 = '';
                    }

                    report.subTotalSum = Math.round(report.amountPrice * 100) / 100;

                    report.amountPrice -= (report.parkingCosts + report.customsFees + report.additionalCosts);
                });
                $scope.totalSum = response.data.totalSum;

                $scope.gridOptions.rowData = response.data.reports;
                if ($scope.gridOptions.api) {
                    $scope.gridOptions.api.onNewRows();
                }
                $scope.columns = $scope.gridOptions.columnDefs;
                $scope.rows = $scope.gridOptions.rowData;

                $scope.gridOptions.api.sizeColumnsToFit();
            });
        //}
    }

    $scope.GetBalanceReportsByContragentID = function () {

        $scope.gridOptions.api.showLoading(true);
        $http.post('/Home/GetBalanceReportsByContragentID', { contragentID: $scope.contrID === null ? $scope.selectedContragent === undefined ? null : $scope.selectedContragent.id : $scope.contrID, year: moment($scope.year).format('YYYY'), month: moment($scope.year).format('MM') }).then(function (response) {
            $scope.gridOptions.rowData = response.data.balances;
                $scope.gridOptions.api.onNewRows();

                $scope.prevSum = response.data.prevSum;
                $scope.currentSum = response.data.currentSum;
                $scope.totalSum = response.data.totalSum;
                $scope.cur = response.data.cur;
                
                $scope.gridOptions.api.sizeColumnsToFit();
        });
    }

    $scope.GetDetailedDriverReportsByContragentID = function () {
        $scope.gridOptions.api.showLoading(true);
        //if ($scope.selectedDriver !== undefined) {
        $http.post('/Home/GetDetailedDriverReportsByContragentID', { driverID: $scope.selectedDriver === undefined ? null : $scope.selectedDriver.id, fromDate: moment($scope.date_from).format('YYYY-MM-DD'), toDate: moment($scope.date_to).format('YYYY-MM-DD'), withCreateDate: $scope.withCreateDate }).then(function (response) {
                angular.forEach(response.data.reports, function (report) {
                    report.dateNow = moment(report.dateNow).format('YYYY-MM-DD') + ' ' + report.startTime + '-' + report.endTime;
                    //report.amountPrice = Math.round((report.amountPrice / 1.18) * 100) / 100;
                    report.parkingCosts = Math.round(report.parkingCosts * 100) / 100;
                    report.customsFees = Math.round(report.customsFees * 100) / 100;
                    report.additionalCosts = Math.round(report.additionalCosts * 100) / 100;
                    
                    if (report.contrVat === '1') {
                        report.amountPrice = Math.round(report.amountPrice * 100) / 100;
                        report.TS = Math.round(report.amountPrice * 100) / 100;
                    }

                    if (report.contrVat === '2') {
                        report.TS = Math.round((report.amountPrice / 1.18) * 100) / 100;
                        report.TSWV1 = Math.round(report.amountPrice * 100) / 100;
                        report.amountPrice = Math.round((report.amountPrice / 1.18) * 100) / 100;
                    } else {
                        report.TSWV1 = '';
                    }

                    if (report.contrVat === '3') {
                        report.TS = roundNumber((report.amountPrice + (report.amountPrice * 0.18)), 1); //(report.amountPrice + (report.amountPrice * 0.18)).toFixed(2);
                        report.TSWV2 = Math.round(report.amountPrice * 100) / 100;
                        report.amountPrice = roundNumber((report.amountPrice + (report.amountPrice * 0.18)), 1); //(report.amountPrice + (report.amountPrice * 0.18)).toFixed(2);
                    } else {
                        report.TSWV2 = '';
                    }

                    report.subTotalSum = Math.round(report.amountPrice * 100) / 100;

                    report.amountPrice -= (report.parkingCosts + report.customsFees + report.additionalCosts);
                });
                $scope.totalSum = response.data.totalSum;

                $scope.gridOptions.rowData = response.data.reports;
                if ($scope.gridOptions.api) {
                    $scope.gridOptions.api.onNewRows();
                }
                $scope.columns = $scope.gridOptions.columnDefs;
                $scope.rows = $scope.gridOptions.rowData;

                $scope.gridOptions.api.sizeColumnsToFit();
            });
        //}
    }

    $scope.GetReportDataByType = function () {
        if ($scope.activeDetailedContragentReport === 'active') {
            if ($scope.XX !== null && $scope.XX !== undefined) {
                $scope.GetDetailedContragentReportsByContragentID();
            }
        }

        if ($scope.activeBalanceReport === 'active') {
            if ($scope.XX !== null && $scope.XX !== undefined) {
                $scope.GetBalanceReportsByContragentID();
            }            
        }

        if ($scope.activeDetailedDriverReport === 'active') {            
            if ($scope.XX !== null && $scope.XX !== undefined) {
                $scope.GetDetailedDriverReportsByContragentID();
            }
        }
    }

    //Excel and PDF generation
    $scope.GenerateExcel = function () {
        if ($scope.activeDetailedContragentReport === 'active') {
            var contragentID = $scope.contrID === null ? $scope.selectedContragent === undefined ? null : $scope.selectedContragent.id : $scope.contrID;
            var fromDate = moment($scope.date_from).format('YYYY-MM-DD');
            var toDate = moment($scope.date_to).format('YYYY-MM-DD');

            window.open(
              '/Home/GenerateDetailedContragentExcel?contragentID=' + contragentID + '&fromDate=' + fromDate + '&toDate=' + toDate,
              '_blank'
            );
        }

        if ($scope.activeDetailedDriverReport === 'active') {
            var driverID = $scope.selectedDriver === undefined ? null : $scope.selectedDriver.id;
            var fromDate = moment($scope.date_from).format('YYYY-MM-DD');
            var toDate = moment($scope.date_to).format('YYYY-MM-DD');
            var withCreateDate = $scope.withCreateDate;

            window.open(
              '/Home/GenerateDetailedDriverExcel?driverID=' + driverID + '&fromDate=' + fromDate + '&toDate=' + toDate + '&withCreateDate=' + withCreateDate,
              '_blank'
            );
        }

        if ($scope.activeBalanceReport === 'active') {
            var contragentID = $scope.contrID === null ? $scope.selectedContragent === undefined ? null : $scope.selectedContragent.id : $scope.contrID;
            var year = moment($scope.year).format('YYYY');
            var month = moment($scope.year).format('MM');

            window.open(
              '/Home/GenerateBalanceExcel?contragentID=' + contragentID + '&year=' + year + '&month=' + month,
              '_blank'
            );
        }
    }

    $scope.GeneratePDF = function () {
        if ($scope.activeDetailedContragentReport === 'active') {
            var contragentID = $scope.contrID === null ? $scope.selectedContragent === undefined ? null : $scope.selectedContragent.id : $scope.contrID;
            var fromDate = moment($scope.date_from).format('YYYY-MM-DD');
            var toDate = moment($scope.date_to).format('YYYY-MM-DD');

            window.open(
              '/Home/GenerateDetailedContragentPdf?contragentID=' + contragentID + '&fromDate=' + fromDate + '&toDate=' + toDate,
              '_blank'
            );
        }

        if ($scope.activeDetailedDriverReport === 'active') {
            var driverID = $scope.selectedDriver === undefined ? null : $scope.selectedDriver.id;
            var fromDate = moment($scope.date_from).format('YYYY-MM-DD');
            var toDate = moment($scope.date_to).format('YYYY-MM-DD');
            var withCreateDate = $scope.withCreateDate;

            window.open(
              '/Home/GenerateDetailedDriverPdf?driverID=' + driverID + '&fromDate=' + fromDate + '&toDate=' + toDate + '&withCreateDate=' + withCreateDate,
              '_blank'
            );
        }

        if ($scope.activeBalanceReport === 'active') {
            var contragentID = $scope.contrID === null ? $scope.selectedContragent === undefined ? null : $scope.selectedContragent.id : $scope.contrID;
            var year = moment($scope.year).format('YYYY');
            var month = moment($scope.year).format('MM');

            window.open(
              '/Home/GenerateBalancePdf?contragentID=' + contragentID + '&year=' + year + '&month=' + month,
              '_blank'
            );
        }
    }
    //---

    $scope.dateOptions = {
        startingDay: 1
    };

    $scope.OpenPwdModal = function () {
        var modalInstance = $modal.open({
            animation: false,
            templateUrl: 'pwdModal.html',
            controller: 'ChangePwdController',
            size: 'md',
            backdrop: true
        });
    }

    $scope.OpenSubReportsModal = function (id, from, to) {
        var modalInstance = $modal.open({
            animation: false,
            templateUrl: 'subReportsModal.html',
            controller: 'SubReportsModalController',
            size: 'lg',
            backdrop: true
        });

        modalInstance.contragentID = id;
        modalInstance.from = from;
        modalInstance.to = to;
    }
});

app.controller('SubReportsModalController', function ($scope, $http, $modalInstance) {
    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
    };

    $scope.contragentID = $modalInstance.contragentID;
    $scope.from = $modalInstance.from;
    $scope.to = $modalInstance.to;

    $scope.init = function () {
        $scope.columnDefs = [
            { headerName: 'მომსახურების დარიცხვის თარიღი', field: 'tDate1' },
            { headerName: 'მომსახურების დაფარვის თარიღი', field: 'tDate2' },
            { headerName: 'დარიცხული თანხა', field: 'amount1' },
            { headerName: 'გადახდილი თანხა', field: 'amount2' },
            { headerName: 'მიმდინარე დავალიანება', field: 'balance' }
        ]

        $scope.gridOptions = {
            columnDefs: $scope.columnDefs,
            rowData: null,
            enableColResize: true,
            rowSelection: 'single',
            enableFilter: true
        };

        $http.post('/Home/GetSubReports', { contragentID: $scope.contragentID }).then(function (response) {
            $scope.gridOptions.api.showLoading(true);

            angular.forEach(response.data.result, function (r) {
                if (r.amount1 === 0) {
                    r.amount1 = '';
                }

                if (r.amount2 === 0) {
                    r.amount2 = '';
                }

                if (r.balance === 0) {
                    r.balance = '';
                }
            });

            $scope.amount1Sum = response.data.amount1Sum;
            $scope.amount2Sum = response.data.amount2Sum;
            $scope.curSum = response.data.curSum;

            $scope.gridOptions.rowData = response.data.result;
            $scope.gridOptions.api.onNewRows();

            $scope.gridOptions.api.sizeColumnsToFit();
        });
    }

    $scope.GenerateSubReportsExcel = function () {
        var contragentID = $scope.contragentID;

        window.open(
          '/Home/GenerateSubReportsExcel?contragentID=' + contragentID,
          '_blank'
        );
    }
});

app.controller('InvoicesController', function ($scope, $http, pickerService, $modal) {
    $scope.init = function () {
        $http.get('/Home/GeneratePage').then(function (response) {
            $scope.XX = response.data.x.x;
            $scope.showHomeMenu = $scope.XX;
            $scope.showContragentsMenu = $scope.XX;
            $scope.showCheckAll = $scope.XX;
            $scope.showSendToEmail = $scope.XX;
            $scope.showDebitAccountsMenu = $scope.XX;

            $scope.showChangePwd = !$scope.XX;
            $scope.isDisabled = $scope.showChangePwd ? '' : 'disabled';

            if ($scope.XX) {
                $scope.chooseYearAndMonth = true;
                if ($scope.gridOptions.api != undefined) {
                    $scope.gridOptions.api.onNewCols();
                }
                $scope.GetInvoicesAdvanced();
            } else {
                $scope.chooseYear = true;
            }
        });

        window.addEventListener('resize', function (event) {
            $('#grdContInvoices').css("height", $(window).height() - 115);
        });
        $('#grdContInvoices').css("height", $(window).height() - 115);

        $http.get('/Home/GetCurrentUser').then(function (response) {
            $scope.fullName = response.data.fullName;
            $scope.contrID = response.data.contragentID;

            if (!$scope.XX) {
                $scope.GetInvoices();
            }
        });
    }

    var dt = moment();

    $scope.openYearpicker = function ($event) {
        $scope.year_opened = true;
    };

    $scope.openYearMonthpicker = function ($event) {
        $scope.yearmonth_opened = true;
    };

    $scope.$watch('year', function (newval, oldval) {
        if (newval !== oldval) {
            if (!$scope.XX) {
                $scope.GetInvoices();
            } else {
                $scope.GetInvoicesAdvanced();
            }
        }
    }, true);

    var filterDate = function (dat) {
        return dat.toDate();
    }

    $scope.year = filterDate(dt);

    $scope.columnDefs = [
        { headerName: 'თვე', field: 'fullDate', headerTooltip: 'თვე' },
        { headerName: 'თანხა დღგ-ს გარეშე', field: 'withoutVat', headerTooltip: 'თანხა დღგ-ს გარეშე' },
        { headerName: 'დღგ', field: 'vat', headerTooltip: 'დღგ' },
        { headerName: 'თანხა დღგ-ს ჩათვლით', field: 'withVat', headerTooltip: 'თანხა დღგ-ს ჩათვლით' },
        {
            headerName: "",
            field: "PDF",
            width: 50,
            suppressSizeToFit: true,
            cellRenderer: function (params) {
                return "<i class='fa fa-file-pdf-o' style='cursor: pointer; display: block; margin-left: 15px; margin-right: auto; margin-top: 5px;' title='PDF'></i>";
            },
            suppressMenu: true,
            suppressSorting: true,
            suppressResize: true
        }
    ]

    $scope.gridOptions = {
        columnDefs: $scope.columnDefs,
        rowData: [],
        enableColResize: true,
        rowSelection: 'multiple',
        enableFilter: true,
        cellClicked: function (cell) {
            if (cell.colDef.field === 'PDF') {
                $scope.GenerateInvoicePdf(cell);
            }

            if (cell.colDef.field === 'isChecked') {
                if (cell.data.isChecked) {
                    cell.data.isChecked = false;
                } else {
                    cell.data.isChecked = true;
                }
            }

            $scope.gridOptions.api.onNewRows();
        }
    }    

    $scope.GetInvoices = function () {
        if ($scope.gridOptions.api != undefined) {
            $scope.gridOptions.api.showLoading(true);
        }
        $http.post('/Home/GetInvoices', { contragentID: $scope.contrID, year: moment($scope.year).format('YYYY') }).then(function (response) {
            angular.forEach(response.data.generalDocs, function (generalDoc) {
                generalDoc.fullDate = moment(generalDoc.tDate).format('MMMM');
            });

            $scope.gridOptions.rowData = response.data.generalDocs;
            $scope.gridOptions.api.onNewRows();

            $scope.gridOptions.api.sizeColumnsToFit();
        });
    }

    $scope.GetInvoicesAdvanced = function () {
        if ($scope.gridOptions.api != undefined) {
            $scope.gridOptions.api.showLoading(true);
        }
        $http.post('/Home/GetInvoicesAdvanced', { year: moment($scope.year).format('YYYY'), month: moment($scope.year).format('MM') }).then(function (response) {
            $scope.columnDefs = [
                {
                    headerName: '',
                    width: 30,
                    hide: true,
                    field: 'isChecked',
                    cellRenderer: function (params) {
                        var result;

                        result = params.value;

                        if (params.value === true) {
                            result = "<i class='fa fa-check-square-o' style='cursor: pointer; display: block; margin-left: 5px; margin-right: auto; margin-top: 5px;'></i>";
                        } else {
                            result = "<i class='fa fa-square-o' style='cursor: pointer; display: block; margin-left: 5px; margin-right: auto; margin-top: 5px;'></i>";
                        }

                        return result;
                    },
                    suppressMenu: true,
                    suppressSorting: true,
                    suppressResize: true
                },
                { headerName: 'კონტრაგენტი', field: 'contragentName', width: 650, headerTooltip: 'კონტრაგენტი' },
                { headerName: 'თანხა დღგ-ს გარეშე', field: 'withoutVat', headerTooltip: 'თანხა დღგ-ს გარეშე' },
                { headerName: 'დღგ', field: 'vat', headerTooltip: 'დღგ' },
                { headerName: 'თანხა დღგ-ს ჩათვლით', field: 'withVat', headerTooltip: 'თანხა დღგ-ს ჩათვლით' },
                {
                    headerName: "",
                    field: "PDF",
                    width: 50,
                    suppressSizeToFit: true,
                    cellRenderer: function (params) {
                        return "<i class='fa fa-file-pdf-o' style='cursor: pointer; display: block; margin-left: 15px; margin-right: auto; margin-top: 5px;' title='PDF'></i>";
                    },
                    suppressMenu: true,
                    suppressSorting: true,
                    suppressResize: true
                }
            ]
            $scope.columnDefs[0].hide = false;
            $scope.gridOptions.columnDefs = $scope.columnDefs;
            $scope.gridOptions.api.onNewCols();

            $scope.gridOptions.rowData = response.data.generalDocs;
            $scope.gridOptions.api.onNewRows();

            $scope.gridOptions.api.sizeColumnsToFit();
        });
    }

    $scope.GenerateInvoicePdf = function (cell) {
        var contragentID = $scope.contrID === null ? cell.data.contragentID : $scope.contrID;
        var year = cell.data.tDate === undefined ? moment($scope.year).format('YYYY') : moment(cell.data.tDate).format('YYYY');
        var month = cell.data.tDate === undefined ? moment($scope.year).format('MM') : moment(cell.data.tDate).format('MM');

        window.open(
          '/Home/GenerateInvoicePdf?contragentID=' + contragentID + '&year=' + year + '&month=' + month,
          '_blank'
        );
    }

    $scope.checkAllInvoices = function () {      
        if ($scope.checkClass === 'fa fa-square-o') {
            $scope.checkClass = 'fa fa-check-square-o';
            angular.forEach($scope.gridOptions.rowData, function (node) {
                node.isChecked = true;

                $scope.gridOptions.api.onNewRows();
            });
        } else {
            $scope.checkClass = 'fa fa-square-o';
            angular.forEach($scope.gridOptions.rowData, function (node) {
                node.isChecked = false;

                $scope.gridOptions.api.onNewRows();
            });
        }
    }

    $scope.checkClass = 'fa fa-square-o';

    $scope.sendToEmailImg = "/Content/Resources/email.png";
    $scope.SendToEmail = function () {
        var selectedContragentIDs = [];
        angular.forEach($scope.gridOptions.rowData, function (rowItem) {
            if (rowItem.isChecked && rowItem.contragentID !== 0) {
                selectedContragentIDs.push(rowItem.contragentID);
            }
        });

        if (selectedContragentIDs.length > 0) {
            var year = moment($scope.year).format('YYYY');
            var month = moment($scope.year).format('MM');

            $scope.sendToEmailImg = "/Content/Resources/lg.gif";
            $http.post('/Home/SendToEmail', { contragentIDs: selectedContragentIDs, year: year, month: month, emailText: $scope.emailText }).then(function (response) {
                $scope.sendToEmailImg = "/Content/Resources/email.png";
            });
        }
    }

    $scope.OpenPwdModal = function () {
        var modalInstance = $modal.open({
            animation: false,
            templateUrl: 'pwdModal.html',
            controller: 'ChangePwdController',
            size: 'md',
            backdrop: true
        });
    }

    $scope.GenerateInvoicesExcel = function () {
        var year = moment($scope.year).format('YYYY');
        var month = moment($scope.year).format('MM');

        window.open(
          '/Home/GenerateInvoicesExcel?year=' + year + '&month=' + month,
          '_blank'
        );
    }
});

app.controller('ContragentsController', function ($scope, $modal, $http, $filter) {
    var columnDefs = [
        { headerName: 'ID', field: 'fakeID', width: 60, suppressSizeToFit: true, headerTooltip: 'ID' },
        { headerName: 'დასახელება', field: 'name', width: 350, headerTooltip: 'დასახელება' },
        { headerName: 'კოდი', field: 'code', width: 200, headerTooltip: 'კოდი' },
        { headerName: 'მისამართი', field: 'address', width: 500, headerTooltip: 'მისამართი' },
        { headerName: 'საკონტაქტო პირი', field: 'contactPerson', width: 200, headerTooltip: 'საკონტაქტო პირი' },
        { headerName: 'ელ-ფოსტა', field: 'email', width: 250, headerTooltip: 'ელ-ფოსტა' },
        { headerName: 'ტელეფონი', field: 'phone', width: 150, headerTooltip: 'ტელეფონი' },
        { headerName: 'დღგ', field: 'vatTypeName', width: 330, headerTooltip: 'დღგ' },
        { headerName: 'ხელშ. გაფორმების თარიღი', field: 'contractStartDate', width: 210, headerTooltip: 'ხელშ. გაფორმების თარიღი' },
        { headerName: 'ხელშ. ამოწურვის თარიღი', field: 'contractExpirationDate', width: 210, headerTooltip: 'ხელშ. ამოწურვის თარიღი' },
        { headerName: 'მომსახურების გადახდის თარიღი', field: 'servicePaymentDate', width: 250, headerTooltip: 'მომსახურების გადახდის რიცხვი' },
        { headerName: 'მომსახურების ტარიფი', field: 'serviceRates', width: 180, headerTooltip: 'მომსახურების ტარიფი' },
        { headerName: 'თვის პერიოდი', field: 'monthPeriod', width: 150, headerTooltip: 'თვის პერიოდი' }
    ]    

    $scope.gridOptions = {
        columnDefs: columnDefs,
        rowData: null,
        enableColResize: true,
        rowSelection: 'single',
        enableFilter: true,
        cellClicked: function (row) {
            $scope.OpenContragentModal('რედაქტირება', row.data);
        }
    }
    
    $scope.init = function () {
        $scope.showChangePwd = false;
        $scope.isDisabled = 'disabled';

        $scope.showNotifyMe = false;
        window.addEventListener('resize', function (event) {
            $('#grdContContrag').css("height", $(window).height() - 115);
        });
        $('#grdContContrag').css("height", $(window).height() - 115);

        $http.get('/Home/GetCurrentUser').then(function (response) {
            $scope.fullName = response.data.fullName;
        });

        app.GetContragentsForGrid = function () {
            $http.get('/Home/GetContragentsForGrid').then(function (response) {
                var dateNow = moment().format('YYYY-MM-DD');
                angular.forEach(response.data.contragents, function (contragent) {
                    switch (contragent.customVat) {
                        case '1':
                            contragent.vatTypeName = 'არ არის გადამხდელი'
                            break;
                        case '2':
                            contragent.vatTypeName = 'გადამხდელი'
                            break;
                        case '3':
                            contragent.vatTypeName = 'განთავისუფლებული ჩათვლის უფლებით'
                            break;
                        case '4':
                            contragent.vatTypeName = 'განთავისუფლებული ჩათვლის უფლების გარეშე'
                            break;
                    }

                    contragent.contractStartDate = contragent.contractStartDate !== null && contragent.contractStartDate !== '' ? moment(contragent.contractStartDate).format('YYYY-MM-DD') : '';
                    contragent.contractExpirationDate = contragent.contractExpirationDate !== null && contragent.contractExpirationDate !== '' ? moment(contragent.contractExpirationDate).format('YYYY-MM-DD') : '';
                });                

                $scope.gridOptions.rowData = response.data.contragents;
                if ($scope.gridOptions.api) {
                    $scope.gridOptions.api.onNewRows();
                }

                $scope.expiredContragents = response.data.expiredContragents;
                if ($scope.expiredContragents.length > 0) {
                    $scope.showNotifyMe = true;
                } else {
                    $scope.showNotifyMe = false;
                }

                $scope.constRowData = response.data.contragents;

                $scope.gridOptions.api.sizeColumnsToFit();
            });            
        }

        app.GetContragentsForGrid();
    }

    var isTicked = false;
    $scope.FilterExpiredContragents = function () {
        if (!isTicked) {
            isTicked = true;
            var filteredData = [];
            angular.forEach($scope.gridOptions.rowData, function (contragent) {
                angular.forEach($scope.expiredContragents, function (expiredContragent) {
                    if (contragent.id === expiredContragent.contrID) {
                        contragent.daysLeft = expiredContragent.daysLeft + ' დღე';
                        filteredData.push(contragent);
                    }
                });
            });
            $scope.gridOptions.rowData = filteredData;
            $scope.gridOptions.api.onNewRows();
            $scope.gridOptions.api.sizeColumnsToFit();
        } else {
            isTicked = false;
            angular.forEach($scope.constRowData, function (contragent) {
                contragent.daysLeft = '';
            });
            $scope.gridOptions.rowData = $scope.constRowData;
            $scope.gridOptions.api.onNewRows();
            $scope.gridOptions.api.sizeColumnsToFit();
        }
    }

    $scope.OpenContragentModal = function (title, rowData) {
        var modalInstance = $modal.open({
            animation: false,
            templateUrl: 'contragentModal.html',
            controller: 'ContragentModalController',
            size: 'lg',
            backdrop: true
        });

        var date = moment().toDate();
        modalInstance.title = title;

        if (rowData === undefined) {
            modalInstance.contractStartDate = date;
            modalInstance.contractExpirationDate = date;
        } else {
            modalInstance.contractStartDate = $filter('date')(rowData.contractStartDate, 'yyyy-MM-dd', '+0400');

            modalInstance.contractExpirationDate = $filter('date')(rowData.contractExpirationDate, 'yyyy-MM-dd', '+0400');            
        }

        modalInstance.servicePaymentDate = rowData === undefined ? '' : rowData.servicePaymentDate;
        modalInstance.id = rowData === undefined ? 0 : rowData.id;
        modalInstance.fakeID = rowData === undefined ? '' : rowData.fakeID;
        modalInstance.name = rowData === undefined ? '' : rowData.name;
        modalInstance.code = rowData === undefined ? '' : rowData.code;
        modalInstance.address = rowData === undefined ? '' : rowData.address;
        modalInstance.email = rowData === undefined ? '' : rowData.email;
        modalInstance.phone = rowData === undefined ? '' : rowData.phone;
        modalInstance.serviceRates = rowData === undefined ? '' : rowData.serviceRates;
        modalInstance.monthPeriod = rowData === undefined ? '' : rowData.monthPeriod;
        modalInstance.customVat = rowData === undefined ? null : rowData.customVat;
        modalInstance.contactPerson = rowData === undefined ? null : rowData.contactPerson;

        modalInstance.daysLeft = rowData === undefined ? '' : rowData.daysLeft === undefined ? '' : rowData.daysLeft;
    }
});

app.controller('DebitAccountsController', function ($scope, $modal, $http, pickerService, $filter) {
    var dt = moment();

    $scope.openFromDatepicker = function ($event) {
        $scope.from_opened = true;
    };

    $scope.openToDatepicker = function ($event) {
        $scope.to_opened = true;
    };

    var filterDate = function (dat) {
        return dat.toDate();
    }

    $scope.date_from = filterDate(dt);
    $scope.date_to = filterDate(dt);

    $scope.$watch('date_from', function (newval, oldval) {
        pickerService.setDates($scope.date_from, $scope.date_to);
        if (newval !== oldval) {
        }
    }, true);

    $scope.$watch('date_to', function (newval, oldval) {
        pickerService.setDates($scope.date_from, $scope.date_to);
        if (newval !== oldval) {
        }
    }, true);

    $scope.menuChoiceClick = function (sign) {
        var year = dt.year();
        switch (sign) {
            case "today":
                $scope.date_from = filterDate(moment());
                $scope.date_to = filterDate(moment());
                break;
            case "year":
                $scope.date_from = filterDate(moment([year, 0, 1]));
                $scope.date_to = filterDate(moment([year, 11, 31]));
                break;
            case "kvartali1":
                $scope.date_from = filterDate(moment([year, 0, 1]));
                $scope.date_to = filterDate(moment([year, 2, 31]));
                break;
            case "kvartali2":
                $scope.date_from = filterDate(moment([year, 3, 1]));
                $scope.date_to = filterDate(moment([year, 5, 30]));
                break;
            case "kvartali3":
                $scope.date_from = filterDate(moment([year, 6, 1]));
                $scope.date_to = filterDate(moment([year, 8, 30]));
                break;
            case "kvartali4":
                $scope.date_from = filterDate(moment([year, 9, 1]));
                $scope.date_to = filterDate(moment([year, 11, 31]));
                break;
            case "yan":
                $scope.date_from = filterDate(moment([year, 0, 1]));
                $scope.date_to = filterDate(moment([year, 0, 31]));
                break;
            case "feb":
                $scope.date_from = filterDate(moment([year, 1, 1]));
                $scope.date_to = filterDate(moment([year, 1, moment([year, 1]).daysInMonth()]));
                break;
            case "mar":
                $scope.date_from = filterDate(moment([year, 2, 1]));
                $scope.date_to = filterDate(moment([year, 2, 31]));
                break;
            case "apr":
                $scope.date_from = filterDate(moment([year, 3, 1]));
                $scope.date_to = filterDate(moment([year, 3, 30]));
                break;
            case "may":
                $scope.date_from = filterDate(moment([year, 4, 1]));
                $scope.date_to = filterDate(moment([year, 4, 31]));
                break
            case "jun":
                $scope.date_from = filterDate(moment([year, 5, 1]));
                $scope.date_to = filterDate(moment([year, 5, 30]));
                break;
            case "jul":
                $scope.date_from = filterDate(moment([year, 6, 1]));
                $scope.date_to = filterDate(moment([year, 6, 31]));
                break;
            case "aug":
                $scope.date_from = filterDate(moment([year, 7, 1]));
                $scope.date_to = filterDate(moment([year, 7, 31]));
                break;
            case "sep":
                $scope.date_from = filterDate(moment([year, 8, 1]));
                $scope.date_to = filterDate(moment([year, 8, 30]));
                break;
            case "oct":
                $scope.date_from = filterDate(moment([year, 9, 1]));
                $scope.date_to = filterDate(moment([year, 9, 31]));
                break;
            case "nov":
                $scope.date_from = filterDate(moment([year, 10, 1]));
                $scope.date_to = filterDate(moment([year, 10, 30]));
                break;
            case "dec":
                $scope.date_from = filterDate(moment([year, 11, 1]));
                $scope.date_to = filterDate(moment([year, 11, 31]));
                break;
        }

        pickerService.setDates($scope.date_from, $scope.date_to);
    };

    var columnDefs = [
        { headerName: 'კონტრაგენტი', field: 'companyName', width: 230, suppressSizeToFit: true },
        { headerName: 'თარიღი', field: 'tDate', width: 120, suppressSizeToFit: true },
        { headerName: 'ბალანსის თარიღი', field: 'tDate1', width: 120, suppressSizeToFit: true },
        { headerName: 'შინაარსი', field: 'purpose' },
        { headerName: 'თანხა', field: 'amount', width: 100, suppressSizeToFit: true },
        { headerName: 'ვალუტა', field: 'currency', width: 150, suppressSizeToFit: true },
        { headerName: 'მომხმარებელი', field: 'userName', width: 300, suppressSizeToFit: true },
        { headerName: 'კომენტარი', field: 'comment', width: 400, suppressSizeToFit: true }
    ];

    $scope.gridOptions = {
        columnDefs: columnDefs,
        rowData: [],
        enableColResize: true,
        rowSelection: 'single',
        enableFilter: true,
        rowClicked: function (row) {
            $scope.OpenDebitAccModal('რედაქტირება', row.data);
        }
    }    

    $scope.init = function () {
        $http.get('/Home/GetCurrentUser').then(function (response) {
            $scope.fullName = response.data.fullName;
            $scope.contrID = response.data.contragentID;
        });

        window.addEventListener('resize', function (event) {
            $('#grdContDebitAccs').css("height", $(window).height() - 115);
        });
        $('#grdContDebitAccs').css("height", $(window).height() - 115);

        app.GetDebitAccounts = function () {
            if ($scope.gridOptions.api !== undefined) {
                $scope.gridOptions.api.showLoading(true);
            }

            $http.post('/Home/GetDebitAccounts', { contragentID: $scope.selectedContragent === undefined ? null : $scope.selectedContragent.id, fromDate: moment($scope.date_from).format('YYYY-MM-DD'), toDate: moment($scope.date_to).format('YYYY-MM-DD') }).then(function (response) {
                angular.forEach(response.data.result, function (r) {
                    r.tDate = moment(r.tDate).format('YYYY-MM-DD');
                    r.tDate1 = moment(r.tDate1).format('YYYY-MM-DD');
                });

                $scope.gridOptions.rowData = response.data.result;
                $scope.gridOptions.api.onNewRows();

                $scope.gridOptions.api.sizeColumnsToFit();
            });
        }        

        $http.get('/Home/GetContragents').then(function (response) {
            angular.forEach(response.data.contragents, function (c) {
                c.name = c.fakeID + ' - ' + c.name;
            });
            $scope.contragents = response.data.contragents;
        });

        $scope.RefreshDebitAccounts = function () {
            app.GetDebitAccounts();
        }
    }

    $scope.dateOptions = {
        startingDay: 1
    };

    $scope.OpenDebitAccModal = function (title, rowData) {
        var modalInstance = $modal.open({
            animation: false,
            templateUrl: 'debitAccountModal.html',
            controller: 'DebitAccountModalController',
            size: 'md',
            backdrop: true
        });

        modalInstance.title = title;

        var date = moment().toDate();

        if (rowData === undefined) {
            modalInstance.tDate = date;
        } else {
            modalInstance.tDate = $filter('date')(rowData.tDate, 'yyyy-MM-dd', '+0400');
        }

        if (rowData === undefined) {
            modalInstance.tDate1 = date;
        } else {
            modalInstance.tDate1 = $filter('date')(rowData.tDate1, 'yyyy-MM-dd', '+0400');
        }

        modalInstance.gdID = rowData === undefined ? 0 : rowData.gdID;
        modalInstance.purpose = rowData === undefined ? '' : rowData.purpose;
        modalInstance.userName = rowData === undefined ? '' : rowData.userName;
        modalInstance.amount = rowData === undefined ? '' : rowData.amount;
        modalInstance.currency = rowData === undefined ? '' : rowData.currency;
        modalInstance.contragentID = rowData === undefined ? null : rowData.contragentID;
        modalInstance.ccaID = rowData === undefined ? 0 : rowData.ccaID;
        modalInstance.curID = rowData === undefined ? 0 : rowData.currency_id;
        modalInstance.comment = rowData === undefined ? '' : rowData.comment;

        modalInstance.contragents = $scope.contragents;
    }
});

app.controller('DebitAccountModalController', function ($scope, $modalInstance, $http) {
    $scope.title = $modalInstance.title;

    $scope.gdID = $modalInstance.gdID;
    $scope.purpose = $modalInstance.purpose;
    $scope.userName = $modalInstance.userName;
    $scope.amount = $modalInstance.amount;
    $scope.currency = $modalInstance.currency;
    $scope.tDate = $modalInstance.tDate;
    $scope.tDate1 = $modalInstance.tDate1;
    $scope.contragentID = $modalInstance.contragentID;
    $scope.ccaID = $modalInstance.ccaID;
    $scope.curID = $modalInstance.curID;
    $scope.comment = $modalInstance.comment;

    if ($scope.title === 'დამატება') {
        $scope.showSaveAndNewButton = true;
    } else {
        $scope.showSaveAndNewButton = false;
    }

    $scope.contragents = $modalInstance.contragents;

    angular.forEach($scope.contragents, function (c) {
        if ($scope.contragentID !== null) {
            if ($scope.contragentID === c.id) {
                $scope.selectedContragent = c;
            }
        }
    });

    $scope.openDatePicker = function ($event) {
        $scope.opened = true;
    };

    $scope.openDatePicker1 = function ($event) {
        $scope.opened1 = true;
    };

    $scope.dateOptions = {
        startingDay: 1
    };

    $scope.dateOptions1 = {
        startingDay: 1
    };

    $scope.init = function () {
        $http.get('/Home/GetCashesAndCompanyAccounts').then(function (response) {
            $scope.CCAs = [];
            angular.forEach(response.data.cashes, function (c) {
                $scope.CCAs.push(c);
            });

            angular.forEach(response.data.companyAccounts, function (ca) {
                ca.id = parseInt(ca.id);
                $scope.CCAs.push(ca);
            });
            
            angular.forEach($scope.CCAs, function (cca) {
                if ($scope.ccaID !== 0) {
                    if ($scope.ccaID === cca.id) {
                        $scope.selectedCCA = cca;
                    }
                } else {
                    $scope.selectedCCA = $scope.CCAs[0];
                }
            });
        });

        $http.get('/Home/GetCurrencies').then(function (response) {
            $scope.curs = response.data.currencies;            

            if ($scope.curID !== 0) {
                angular.forEach($scope.curs, function (c) {
                    if ($scope.curID === c.id) {
                        $scope.selectedCur = c;
                    }
                });                
            } else {
                $scope.selectedCur = $scope.curs[0];
            }
        });        
    }    

    $scope.selectCCA = function (CCAIndex) {
        $scope.selectedCCA = $scope.CCAs[CCAIndex];

        $scope.validateModal();
    }    

    $scope.selectCur = function (CurIndex) {
        $scope.selectedCur = $scope.curs[CurIndex];

        $scope.validateModal();
    }

    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
    };

    $scope.validateModal = function () {
        var result = true;

        result = result && IsNullOrEmpty($scope.tDate);
        $scope.validateInputs($scope.tDate, $scope.tDate1, $scope.selectedContragent, $scope.selectedCCA, $scope.amount, $scope.selectedCur);

        result = result && IsNullOrEmpty($scope.tDate1);
        $scope.validateInputs($scope.tDate, $scope.tDate1, $scope.selectedContragent, $scope.selectedCCA, $scope.amount, $scope.selectedCur);

        result = result && IsNullOrEmpty($scope.selectedContragent);
        $scope.validateInputs($scope.tDate, $scope.tDate1, $scope.selectedContragent, $scope.selectedCCA, $scope.amount, $scope.selectedCur);

        result = result && IsNullOrEmpty($scope.selectedCCA);
        $scope.validateInputs($scope.tDate, $scope.tDate1, $scope.selectedContragent, $scope.selectedCCA, $scope.amount, $scope.selectedCur);

        result = result && IsNullOrEmpty($scope.amount);
        $scope.validateInputs($scope.tDate, $scope.tDate1, $scope.selectedContragent, $scope.selectedCCA, $scope.amount, $scope.selectedCur);

        result = result && IsNullOrEmpty($scope.selectedCur);
        $scope.validateInputs($scope.tDate, $scope.tDate1, $scope.selectedContragent, $scope.selectedCCA, $scope.amount, $scope.selectedCur);

        return result;
    }

    $scope.validateInputs = function (cldDate, cldDate1, thContr, cmbACC, txtAmount, cmbCur) {
        if (IsNullOrEmpty(cldDate)) {
            document.getElementById('cldDate').style.backgroundColor = "#fff";
        } else {
            document.getElementById('cldDate').style.backgroundColor = "#ffb7b7";
        }

        if (IsNullOrEmpty(cldDate1)) {
            document.getElementById('cldDate1').style.backgroundColor = "#fff";
        } else {
            document.getElementById('cldDate1').style.backgroundColor = "#ffb7b7";
        }

        if (IsNullOrEmpty(thContr)) {
            document.getElementById('thContr').style.backgroundColor = "#fff";
        } else {
            document.getElementById('thContr').style.backgroundColor = "#ffb7b7";
        }

        if (IsNullOrEmpty(cmbACC)) {
            document.getElementsByClassName('cmbACC')[0].style.backgroundColor = "#fff";
        } else {
            document.getElementsByClassName('cmbACC')[0].style.backgroundColor = "#ffb7b7";
        }

        if (IsNullOrEmpty(txtAmount)) {
            document.getElementById('txtAmount').style.backgroundColor = "#fff";
        } else {
            document.getElementById('txtAmount').style.backgroundColor = "#ffb7b7";
        }

        if (IsNullOrEmpty(cmbCur)) {
            document.getElementsByClassName('cmbCur')[0].style.backgroundColor = "#fff";
        } else {
            document.getElementsByClassName('cmbCur')[0].style.backgroundColor = "#ffb7b7";
        }
    }

    $scope.RestoreDefaults = function () {
        $scope.gdID = 0;
        $scope.tDate = new Date();
        $scope.tDate1 = new Date();
        $scope.selectedContragent = null;
        $scope.selectedCCA = null;
        $scope.amount = '';
        $scope.selectedCur = null;
        $scope.comment = '';
    }

    $scope.Save = function () {
        if ($scope.validateModal()) {
            $http.post('/Home/SaveDebitAccount', { gdID: $scope.gdID, amount: parseFloat($scope.amount), currencyID: $scope.selectedCur.id, contragentID: $scope.selectedContragent.id, CCAID: $scope.selectedCCA.id, tDate: $scope.tDate, tDate1: $scope.tDate1, comment: $scope.comment }).then(function (response) {
                if (response.data.saveResult) {
                    $scope.showSuccessAlert = true;
                    $scope.showFailureAlert = false;

                    setTimeout(function () {
                        $scope.Cancel();

                        app.GetDebitAccounts();
                    }, 1000);
                } else {
                    $scope.showSuccessAlert = false;
                    $scope.showFailureAlert = true;
                }
            }, function (response) {
                $scope.showSuccessAlert = false;
                $scope.showFailureAlert = true;
            });
        }
    }

    $scope.SaveAndNew = function () {
        if ($scope.validateModal()) {
            $http.post('/Home/SaveDebitAccount', { gdID: $scope.gdID, amount: parseFloat($scope.amount), currencyID: $scope.selectedCur.id, contragentID: $scope.selectedContragent.id, CCAID: $scope.selectedCCA.id, tDate: $scope.tDate, tDate1: $scope.tDate1, comment: $scope.comment }).then(function (response) {
                if (response.data.saveResult) {
                    $scope.showSuccessAlert = true;
                    $scope.showFailureAlert = false;

                    setTimeout(function () {
                        $scope.showSuccessAlert = false;
                        $scope.showFailureAlert = false;

                        $scope.RestoreDefaults();

                        app.GetDebitAccounts();
                    }, 1000);
                } else {
                    $scope.showSuccessAlert = false;
                    $scope.showFailureAlert = true;
                }
            }, function (response) {
                $scope.showSuccessAlert = false;
                $scope.showFailureAlert = true;
            });
        }
    }
});

app.controller('ContragentModalController', function ($scope, $modalInstance, $http) {
    $scope.title = $modalInstance.title;

    $scope.id = $modalInstance.id;
    $scope.fakeID = $modalInstance.fakeID;
    $scope.name = $modalInstance.name;
    $scope.code = $modalInstance.code;
    $scope.address = $modalInstance.address;
    $scope.email = $modalInstance.email;
    $scope.phone = $modalInstance.phone;
    $scope.contractStartDate = $modalInstance.contractStartDate;
    $scope.contractExpirationDate = $modalInstance.contractExpirationDate;
    $scope.servicePaymentDate = $modalInstance.servicePaymentDate;
    $scope.serviceRates = $modalInstance.serviceRates;
    $scope.monthPeriod = $modalInstance.monthPeriod;
    $scope.selectedVatTypeValue = parseInt($modalInstance.customVat);
    $scope.contactPerson = $modalInstance.contactPerson;

    $scope.daysLeft = $modalInstance.daysLeft;

    if ($scope.daysLeft !== undefined && $scope.daysLeft !== null && $scope.daysLeft !== '') {
        if (parseInt($scope.daysLeft) <= 14) {
            $scope.expDateStyle = 'background-color: #FFFF99;';
        }
    }

    $scope.vatTypes = ['არ არის გადამხდელი', 'გადამხდელი', 'განთავისუფლებული ჩათვლის უფლებით', 'განთავისუფლებული ჩათვლის უფლების გარეშე'];
    $scope.selectedVatType = $scope.selectedVatTypeValue === null ? '' : $scope.vatTypes[$scope.selectedVatTypeValue - 1];

    if ($scope.title === 'დამატება') {
        $scope.showSaveAndNewButton = true;
        $scope.showPwdReturn = false;
    } else {
        $scope.showSaveAndNewButton = false;
        $scope.showPwdReturn = true;
    }

    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
    };

    $scope.RestoreDefaults = function () {
        $scope.id = 0;
        $scope.fakeID = '';
        $scope.name = '';
        $scope.code = '';
        $scope.address = '';
        $scope.email = '';
        $scope.phone = '';
        $scope.contractStartDate = new Date();
        $scope.contractExpirationDate = new Date();
        $scope.servicePaymentDate = new Date();
        $scope.serviceRates = '';
        $scope.monthPeriod = '';
        $scope.contactPerson = '';

        $scope.selectedVatType = '';
        $scope.selectedVatTypeValue = null;
    }

    $scope.Save = function () {
        if ($scope.validateModal()) {
            var contragentModel = {
                id: $scope.id,
                fakeID: $scope.fakeID,
                name: $scope.name,
                code: $scope.code,
                address: $scope.address,
                email: $scope.email,
                phone: $scope.phone,
                contractStartDate: $scope.contractStartDate,
                contractExpirationDate: $scope.contractExpirationDate,
                servicePaymentDate: $scope.servicePaymentDate,
                serviceRates: $scope.serviceRates,
                monthPeriod: $scope.monthPeriod,
                customVat: $scope.selectedVatTypeValue,
                contactPerson: $scope.contactPerson
            };

            $http.post('/Home/SaveContragent', { contragentModel: contragentModel }).then(function (response) {
                if (response.data.saveResult) {
                    $scope.showSuccessAlert = true;
                    $scope.showFailureAlert = false;

                    setTimeout(function () {
                        $scope.Cancel();

                        app.GetContragentsForGrid();
                    }, 1000);
                } else {
                    $scope.showSuccessAlert = false;
                    $scope.showFailureAlert = true;
                }
            }, function (response) {
                $scope.showSuccessAlert = false;
                $scope.showFailureAlert = true;
            });
        }
    }

    $scope.SaveAndNew = function () {
        if ($scope.validateModal()) {
            var contragentModel = {
                id: $scope.id,
                fakeID: $scope.fakeID,
                name: $scope.name,
                code: $scope.code,
                address: $scope.address,
                email: $scope.email,
                phone: $scope.phone,
                contractStartDate: $scope.contractStartDate,
                contractExpirationDate: $scope.contractExpirationDate,
                servicePaymentDate: $scope.servicePaymentDate,
                serviceRates: $scope.serviceRates,
                monthPeriod: $scope.monthPeriod,
                customVat: $scope.selectedVatTypeValue,
                contactPerson: $scope.contactPerson
            };

            $http.post('/Home/SaveContragent', { contragentModel: contragentModel }).then(function (response) {
                if (response.data.saveResult) {
                    $scope.showSuccessAlert = true;
                    $scope.showFailureAlert = false;

                    setTimeout(function () {
                        $scope.showSuccessAlert = false;
                        $scope.showFailureAlert = false;

                        $scope.RestoreDefaults();

                        app.GetContragentsForGrid();
                    }, 1000);
                } else {
                    $scope.showSuccessAlert = false;
                    $scope.showFailureAlert = true;
                }
            }, function (response) {
                $scope.showSuccessAlert = false;
                $scope.showFailureAlert = true;
            });
        }
    }

    $scope.ReturnPwd = function () {
        $http.post('/Home/ReturnPwd', { contragentID: $scope.id }).then(function (response) {
            if (response.data.saveResult) {
                $scope.showSuccessAlert1 = true;
                $scope.showFailureAlert1 = false;

                setTimeout(function () {
                    $scope.Cancel();
                }, 1000);
            } else {
                $scope.showSuccessAlert1 = false;
                $scope.showFailureAlert1 = true;
            }
        }, function (response) {
            $scope.showSuccessAlert1 = false;
            $scope.showFailureAlert1 = true;
        });
    }
    
    $scope.openContractStartDatePicker = function ($event) {
        $scope.openedContractStart = true;
    };

    $scope.openContractExpirationDatePicker = function ($event) {
        $scope.openedContractExpiration = true;
    };

    $scope.dateOptions = {
        startingDay: 1
    };

    $scope.selectVatType = function (vatTypeIndex) {
        $scope.selectedVatType = $scope.vatTypes[vatTypeIndex];
        $scope.selectedVatTypeValue = vatTypeIndex + 1;

        $scope.ValidateOtherRequiredInputs();
    }

    $scope.validateInputs = function (txtID, txtName, txtCode, txtAddress, txtContactPerson, txtEmail, txtPhone, dt1, dt2, dt3, cmbVatType, txtServiceRates, txtVat) {
        if (IsNullOrEmpty(txtID)) {
            document.getElementById('txtID').style.backgroundColor = "#fff";
        } else {
            document.getElementById('txtID').style.backgroundColor = "#ffb7b7";
        }

        if (IsNullOrEmpty(txtName)) {
            document.getElementById('txtName').style.backgroundColor = "#fff";
        } else {
            document.getElementById('txtName').style.backgroundColor = "#ffb7b7";
        }

        if (IsNullOrEmpty(txtCode)) {
            document.getElementById('txtCode').style.backgroundColor = "#fff";
        } else {
            document.getElementById('txtCode').style.backgroundColor = "#ffb7b7";
        }

        if (IsNullOrEmpty(txtAddress)) {
            document.getElementById('txtAddress').style.backgroundColor = "#fff";
        } else {
            document.getElementById('txtAddress').style.backgroundColor = "#ffb7b7";
        }

        if (IsNullOrEmpty(txtContactPerson)) {
            document.getElementById('txtContactPerson').style.backgroundColor = "#fff";
        } else {
            document.getElementById('txtContactPerson').style.backgroundColor = "#ffb7b7";
        }

        if (IsNullOrEmpty(txtEmail)) {
            document.getElementById('txtEmail').style.backgroundColor = "#fff";
        } else {
            document.getElementById('txtEmail').style.backgroundColor = "#ffb7b7";
        }

        if (IsNullOrEmpty(txtPhone)) {
            document.getElementById('txtPhone').style.backgroundColor = "#fff";
        } else {
            document.getElementById('txtPhone').style.backgroundColor = "#ffb7b7";
        }

        if (IsNullOrEmpty(dt1)) {
            document.getElementById('dt1').style.backgroundColor = "#fff";
        } else {
            document.getElementById('dt1').style.backgroundColor = "#ffb7b7";
        }

        if (IsNullOrEmpty(dt2)) {
            document.getElementById('dt2').style.backgroundColor = "#fff";
        } else {
            document.getElementById('dt2').style.backgroundColor = "#ffb7b7";
        }

        if (IsNullOrEmpty(dt3)) {
            document.getElementById('dt3').style.backgroundColor = "#fff";
        } else {
            document.getElementById('dt3').style.backgroundColor = "#ffb7b7";
        }

        if (IsNullOrEmpty(cmbVatType)) {
            document.getElementsByClassName('cmbVatType')[0].style.backgroundColor = "#fff";
        } else {
            document.getElementsByClassName('cmbVatType')[0].style.backgroundColor = "#ffb7b7";
        }

        if (IsNullOrEmpty(txtServiceRates)) {
            document.getElementById('txtServiceRates').style.backgroundColor = "#fff";
        } else {
            document.getElementById('txtServiceRates').style.backgroundColor = "#ffb7b7";
        }
    }

    $scope.validateModal = function () {
        var result = true;

        result = result && IsNullOrEmpty($scope.fakeID);
        $scope.validateInputs($scope.fakeID, $scope.name, $scope.code, $scope.address, $scope.contactPerson, $scope.email, $scope.phone, $scope.contractStartDate, $scope.contractExpirationDate, $scope.servicePaymentDate, $scope.selectedVatType, $scope.serviceRates, $scope.isVatAddMinusOrDefault);

        result = result && IsNullOrEmpty($scope.name);
        $scope.validateInputs($scope.fakeID, $scope.name, $scope.code, $scope.address, $scope.contactPerson, $scope.email, $scope.phone, $scope.contractStartDate, $scope.contractExpirationDate, $scope.servicePaymentDate, $scope.selectedVatType, $scope.serviceRates, $scope.isVatAddMinusOrDefault);

        result = result && IsNullOrEmpty($scope.code);
        $scope.validateInputs($scope.fakeID, $scope.name, $scope.code, $scope.address, $scope.contactPerson, $scope.email, $scope.phone, $scope.contractStartDate, $scope.contractExpirationDate, $scope.servicePaymentDate, $scope.selectedVatType, $scope.serviceRates, $scope.isVatAddMinusOrDefault);

        result = result && IsNullOrEmpty($scope.address);
        $scope.validateInputs($scope.fakeID, $scope.name, $scope.code, $scope.address, $scope.contactPerson, $scope.email, $scope.phone, $scope.contractStartDate, $scope.contractExpirationDate, $scope.servicePaymentDate, $scope.selectedVatType, $scope.serviceRates, $scope.isVatAddMinusOrDefault);

        result = result && IsNullOrEmpty($scope.contactPerson);
        $scope.validateInputs($scope.fakeID, $scope.name, $scope.code, $scope.address, $scope.contactPerson, $scope.email, $scope.phone, $scope.contractStartDate, $scope.contractExpirationDate, $scope.servicePaymentDate, $scope.selectedVatType, $scope.serviceRates, $scope.isVatAddMinusOrDefault);

        result = result && IsNullOrEmpty($scope.email);
        $scope.validateInputs($scope.fakeID, $scope.name, $scope.code, $scope.address, $scope.contactPerson, $scope.email, $scope.phone, $scope.contractStartDate, $scope.contractExpirationDate, $scope.servicePaymentDate, $scope.selectedVatType, $scope.serviceRates, $scope.isVatAddMinusOrDefault);

        result = result && IsNullOrEmpty($scope.phone);
        $scope.validateInputs($scope.fakeID, $scope.name, $scope.code, $scope.address, $scope.contactPerson, $scope.email, $scope.phone, $scope.contractStartDate, $scope.contractExpirationDate, $scope.servicePaymentDate, $scope.selectedVatType, $scope.serviceRates, $scope.isVatAddMinusOrDefault);

        result = result && IsNullOrEmpty($scope.contractStartDate);
        $scope.validateInputs($scope.fakeID, $scope.name, $scope.code, $scope.address, $scope.contactPerson, $scope.email, $scope.phone, $scope.contractStartDate, $scope.contractExpirationDate, $scope.servicePaymentDate, $scope.selectedVatType, $scope.serviceRates, $scope.isVatAddMinusOrDefault);

        result = result && IsNullOrEmpty($scope.contractExpirationDate);
        $scope.validateInputs($scope.fakeID, $scope.name, $scope.code, $scope.address, $scope.contactPerson, $scope.email, $scope.phone, $scope.contractStartDate, $scope.contractExpirationDate, $scope.servicePaymentDate, $scope.selectedVatType, $scope.serviceRates, $scope.isVatAddMinusOrDefault);

        result = result && IsNullOrEmpty($scope.servicePaymentDate);
        $scope.validateInputs($scope.fakeID, $scope.name, $scope.code, $scope.address, $scope.contactPerson, $scope.email, $scope.phone, $scope.contractStartDate, $scope.contractExpirationDate, $scope.servicePaymentDate, $scope.selectedVatType, $scope.serviceRates, $scope.isVatAddMinusOrDefault);

        result = result && IsNullOrEmpty($scope.selectedVatType);
        $scope.validateInputs($scope.fakeID, $scope.name, $scope.code, $scope.address, $scope.contactPerson, $scope.email, $scope.phone, $scope.contractStartDate, $scope.contractExpirationDate, $scope.servicePaymentDate, $scope.selectedVatType, $scope.serviceRates, $scope.isVatAddMinusOrDefault);

        result = result && IsNullOrEmpty($scope.serviceRates);
        $scope.validateInputs($scope.fakeID, $scope.name, $scope.code, $scope.address, $scope.contactPerson, $scope.email, $scope.phone, $scope.contractStartDate, $scope.contractExpirationDate, $scope.servicePaymentDate, $scope.selectedVatType, $scope.serviceRates, $scope.isVatAddMinusOrDefault);

        return result;
    }

    $scope.ValidateOtherRequiredInputs = function () {
        $scope.validateInputs($scope.fakeID, $scope.name, $scope.code, $scope.address, $scope.contactPerson, $scope.email, $scope.phone, $scope.contractStartDate, $scope.contractExpirationDate, $scope.servicePaymentDate, $scope.selectedVatType, $scope.serviceRates, $scope.isVatAddMinusOrDefault);
    }
});

app.controller('ChangePwdController', function ($scope, $modalInstance, $http) {
    $scope.Cancel = function () {
        $modalInstance.dismiss('cancel');
    };

    $scope.validateInputs = function (txtOldPwd, txtNewPwd, txtRepeatNewPwd) {
        if (IsNullOrEmpty(txtOldPwd)) {
            document.getElementById('txtOldPwd').style.backgroundColor = "#fff";
        } else {
            document.getElementById('txtOldPwd').style.backgroundColor = "#ffb7b7";
        }

        if (IsNullOrEmpty(txtNewPwd)) {
            document.getElementById('txtNewPwd').style.backgroundColor = "#fff";
        } else {
            document.getElementById('txtNewPwd').style.backgroundColor = "#ffb7b7";
        }

        if (IsNullOrEmpty(txtRepeatNewPwd)) {
            document.getElementById('txtRepeatNewPwd').style.backgroundColor = "#fff";
        } else {
            document.getElementById('txtRepeatNewPwd').style.backgroundColor = "#ffb7b7";
        }
    }

    $scope.validateModal = function () {
        $scope.showNotEqualsAlert = false;
        $scope.showErrorMsgAlert = false;
        $scope.showLengthAlert = false;
        var result = true;

        result = result && IsNullOrEmpty($scope.oldPwd);
        $scope.validateInputs($scope.oldPwd, $scope.newPwd, $scope.repeatNewPwd);

        result = result && IsNullOrEmpty($scope.newPwd);
        $scope.validateInputs($scope.oldPwd, $scope.newPwd, $scope.repeatNewPwd);

        result = result && IsNullOrEmpty($scope.repeatNewPwd);
        $scope.validateInputs($scope.oldPwd, $scope.newPwd, $scope.repeatNewPwd);

        return result;
    }

    $scope.Save = function () {
        if ($scope.validateModal()) {
            if ($scope.newPwd === $scope.repeatNewPwd) {
                if ($scope.newPwd.length > 0 && $scope.newPwd.length <= 12) {
                    $http.post('/Home/ChangeContragentPwd', { oldPwd: $scope.oldPwd, newPwd: $scope.newPwd, repeatNewPwd: $scope.repeatNewPwd }).then(function (response) {
                        if (!response.data.result && response.data.errorMsg !== '') {
                            $scope.errorMsg = response.data.errorMsg;
                            $scope.showErrorMsgAlert = true;
                        } else {
                            $scope.showSuccessAlert = true;
                            $scope.showFailureAlert = false;

                            setTimeout(function () {
                                $scope.Cancel();
                                LogOutSafe();
                            }, 1000);
                        }
                    }, function (response) {
                        $scope.showSuccessAlert = false;
                        $scope.showFailureAlert = true;
                    });
                } else {
                    $scope.showLengthAlert = true;
                }
            } else {
                $scope.showNotEqualsAlert = true;
            }
        }
    }
});

app.controller('DriverSalaryReportController', function ($scope, $http, pickerService) {
    var columnDefs = [
        { headerName: 'მძღოლი', field: 'staffName' },
        { headerName: 'თანხა Web', field: 'amountPrice1', suppressSizeToFit: true, width: 200 },
        { headerName: 'თანხა FINA', field: 'amountPrice2', suppressSizeToFit: true, width: 200 },        
        { headerName: 'ჯამი', field: 'amountSum', suppressSizeToFit: true, width: 250 }
    ];

    $scope.gridOptions = {
        columnDefs: columnDefs,
        rowData: [],
        enableColResize: true,
        rowSelection: 'single',
        enableFilter: true
    }

    $scope.init = function () {
        $scope.isDisabled = 'disabled';
        $http.get('/Home/GetCurrentUser').then(function (response) {
            $scope.fullName = response.data.fullName;
            $scope.contrID = response.data.contragentID;
        });

        window.addEventListener('resize', function (event) {
            $('#grdContDriverSalary').css("height", $(window).height() - 145);
        });
        $('#grdContDriverSalary').css("height", $(window).height() - 145);

        var dt = moment();

        $scope.openFromDatepicker = function ($event) {
            $scope.from_opened = true;
        };

        $scope.openToDatepicker = function ($event) {
            $scope.to_opened = true;
        };

        var filterDate = function (dat) {
            return dat.toDate();
        }

        $scope.date_from = filterDate(dt);
        $scope.date_to = filterDate(dt);

        $scope.$watch('date_from', function (newval, oldval) {
            pickerService.setDates($scope.date_from, $scope.date_to);
            if (newval !== oldval) {
            }
        }, true);

        $scope.$watch('date_to', function (newval, oldval) {
            pickerService.setDates($scope.date_from, $scope.date_to);
            if (newval !== oldval) {
            }
        }, true);

        $scope.menuChoiceClick = function (sign) {
            var year = dt.year();
            switch (sign) {
                case "today":
                    $scope.date_from = filterDate(moment());
                    $scope.date_to = filterDate(moment());
                    break;
                case "year":
                    $scope.date_from = filterDate(moment([year, 0, 1]));
                    $scope.date_to = filterDate(moment([year, 11, 31]));
                    break;
                case "kvartali1":
                    $scope.date_from = filterDate(moment([year, 0, 1]));
                    $scope.date_to = filterDate(moment([year, 2, 31]));
                    break;
                case "kvartali2":
                    $scope.date_from = filterDate(moment([year, 3, 1]));
                    $scope.date_to = filterDate(moment([year, 5, 30]));
                    break;
                case "kvartali3":
                    $scope.date_from = filterDate(moment([year, 6, 1]));
                    $scope.date_to = filterDate(moment([year, 8, 30]));
                    break;
                case "kvartali4":
                    $scope.date_from = filterDate(moment([year, 9, 1]));
                    $scope.date_to = filterDate(moment([year, 11, 31]));
                    break;
                case "yan":
                    $scope.date_from = filterDate(moment([year, 0, 1]));
                    $scope.date_to = filterDate(moment([year, 0, 31]));
                    break;
                case "feb":
                    $scope.date_from = filterDate(moment([year, 1, 1]));
                    $scope.date_to = filterDate(moment([year, 1, moment([year, 1]).daysInMonth()]));
                    break;
                case "mar":
                    $scope.date_from = filterDate(moment([year, 2, 1]));
                    $scope.date_to = filterDate(moment([year, 2, 31]));
                    break;
                case "apr":
                    $scope.date_from = filterDate(moment([year, 3, 1]));
                    $scope.date_to = filterDate(moment([year, 3, 30]));
                    break;
                case "may":
                    $scope.date_from = filterDate(moment([year, 4, 1]));
                    $scope.date_to = filterDate(moment([year, 4, 31]));
                    break
                case "jun":
                    $scope.date_from = filterDate(moment([year, 5, 1]));
                    $scope.date_to = filterDate(moment([year, 5, 30]));
                    break;
                case "jul":
                    $scope.date_from = filterDate(moment([year, 6, 1]));
                    $scope.date_to = filterDate(moment([year, 6, 31]));
                    break;
                case "aug":
                    $scope.date_from = filterDate(moment([year, 7, 1]));
                    $scope.date_to = filterDate(moment([year, 7, 31]));
                    break;
                case "sep":
                    $scope.date_from = filterDate(moment([year, 8, 1]));
                    $scope.date_to = filterDate(moment([year, 8, 30]));
                    break;
                case "oct":
                    $scope.date_from = filterDate(moment([year, 9, 1]));
                    $scope.date_to = filterDate(moment([year, 9, 31]));
                    break;
                case "nov":
                    $scope.date_from = filterDate(moment([year, 10, 1]));
                    $scope.date_to = filterDate(moment([year, 10, 30]));
                    break;
                case "dec":
                    $scope.date_from = filterDate(moment([year, 11, 1]));
                    $scope.date_to = filterDate(moment([year, 11, 31]));
                    break;
            }

            pickerService.setDates($scope.date_from, $scope.date_to);
        };

        $scope.dateOptions = {
            startingDay: 1
        };

        app.GetDriverSalaryReports = function () {
            $scope.gridOptions.api.showLoading(true);
            $http.post('/Home/GetDriverSalaryReports', { fromDate: moment($scope.date_from).format('YYYY-MM-DD'), toDate: moment($scope.date_to).format('YYYY-MM-DD') }).then(function (response) {
                $scope.gridOptions.rowData = response.data.reports;                
                $scope.gridOptions.api.onNewRows();
                $scope.totalAmountSum = response.data.totalAmountSum;
            });
            $scope.gridOptions.api.sizeColumnsToFit();
        }

        $scope.RefreshDriverSalaryReports = function () {
            app.GetDriverSalaryReports();
        }

        $scope.GenerateDriverSalaryReportExcel = function () {
            var fromDate = moment($scope.date_from).format('YYYY-MM-DD');
            var toDate = moment($scope.date_to).format('YYYY-MM-DD');

            window.open(
              '/Home/GenerateDriverSalaryReportExcel?fromDate=' + fromDate + '&toDate=' + toDate,
              '_blank'
            );
        }
    }
});