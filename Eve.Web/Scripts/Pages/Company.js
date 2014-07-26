function CompanyModel(data){
    var self  = this;
    this.CompanyId = data.CompanyId;
    this.CompanyName = ko.observable(data.CompanyName);
    this.CompanyDescription = ko.observable(data.CompanyDescription);
    this.CompanyLogo = ko.observable(data.CompanyLogo);
    this.CompanyIsActive = ko.observable(data.CompanyIsActive);
    this.originalData = data;

    this.CompanyLogoFull = ko.computed(function(){
        return '../Content/Images/Companies/' + self.CompanyLogo();
    })

    this.beginEdit = function(){
        model.SelectedCompany(self);
        setTimeout(applyLogoUpload, 250);
    }
    this.cancelEdit = function(){
        self.CompanyName(self.originalData.CompanyName);
        self.CompanyDescription(self.originalData.CompanyDescription);
        self.CompanyLogo(self.originalData.CompanyLogo);
        self.CompanyIsActive(self.originalData.CompanyIsActive);
        model.SelectedCompany(null);
    }
    this.saveEdit = function(){
        $.ajax({
            url: '../Company/Save',
            type: 'POST',
            data: {
                CompanyId: self.CompanyId,
                CompanyName: self.CompanyName(),
                CompanyDescription: self.CompanyDescription(),
                CompanyLogo: self.CompanyLogo(),
                CompanyIsActive: self.CompanyIsActive()
            },
            success: function(r){
                model.SelectedCompany(null);
                model.refreshGrid();
            },
            error: function(a, b, c){

            }
        })
    }
}
function ViewModel(){
    var self = this;
    this.Companies = ko.observableArray([]);
    this.SelectedCompany = ko.observable();
    this.gridOptions = {
        RowsPerPage: ko.observable(25),
        CurrentPage: ko.observable(0),
        OrderBy: ko.observable('CompanyId'),
        OrderDescending: ko.observable(false),
        TotalCount: ko.observable(0),
        PageSizes: [5, 10, 25, 50],
        TotalPages: ko.observable(0),
        Pages: ko.observableArray([])
    }
    this.filterLike = ko.observable('');
    this.SUPRESS = false;

    this.getCount = function(){
        $.ajax({
            url: '../Company/GetCount',
            data: { like: self.filterLike() },
            type: 'POST',
            success: function(r){
                self.gridOptions.TotalCount(Number(r));
                var total = Math.ceil(r / self.gridOptions.RowsPerPage());
                self.gridOptions.TotalPages(total);
            },
            error: function(a, b, c){

            }
        })
    }
    this.getElements = function(){
        $.ajax({
            url: '../Company/Get',
            data: JSON.stringify({
                options: {
                    CurrentPage: self.gridOptions.CurrentPage(),
                    RowsPerPage: self.gridOptions.RowsPerPage(),
                    OrderBy: self.gridOptions.OrderBy(),
                    OrderDescending: self.gridOptions.OrderDescending()
                },
                like: self.filterLike()
            }),
            contentType: 'application/json',
            type: 'POST',
            success: function(r){
                self.Companies.removeAll();
                for(var i = 0; i < r.length; i++)
                    self.Companies.push(new CompanyModel(r[i]));
            },
            error: function(a, b, c){

            }
        })
    }
    this.setSortMode = function(orderBy){
        if(self.gridOptions.OrderBy() == orderBy){
            self.gridOptions.OrderDescending(!self.gridOptions.OrderDescending())
        }
        else{
            self.gridOptions.OrderBy(orderBy);
            self.gridOptions.OrderDescending(false);
        }
        self.getElements();
        handleSortChange();
    }
    this.refreshGrid = function(){
        self.getCount();
        self.getElements();
        handleSortChange();
    }
    this.newCompany = function(){
        self.SelectedCompany(new CompanyModel({
            CompanyId: -1,
            CompanyName: '',
            CompanyDescription: '',
            CompanyLogo: '',
            CompanyIsActive: true
        }));
        setTimeout(applyLogoUpload, 250);
    }

    this.filterLike.subscribe(function(){
        var c = self.gridOptions.CurrentPage();
        if(c == 0)
            self.refreshGrid();
        else
            self.gridOptions.CurrentPage(0);
    });
    this.gridOptions.RowsPerPage.subscribe(function(){
        self.refreshGrid();
    });
    this.gridOptions.CurrentPage.subscribe(function(){
        if(!self.SUPRESS)
            self.refreshGrid();
    });
    this.footerNavigation = ko.computed(function(){
        var total = self.gridOptions.TotalPages();
        var panel = document.createElement('div');
        
            var btnPrev = document.createElement('button');
            btnPrev.className = 'btn btn-default';
            btnPrev.innerHTML = '&lt;'
            btnPrev.disabled = self.gridOptions.CurrentPage() <= 0;
            btnPrev.onclick = function(){
                self.gridOptions.CurrentPage(self.gridOptions.CurrentPage() - 1);
            }
            panel.appendChild(btnPrev);

            var btnNext = document.createElement('button');
            btnNext.className = 'btn btn-default';
            btnNext.innerHTML = '&gt;'
            btnNext.disabled = self.gridOptions.CurrentPage() + 1 >= total;
            btnNext.onclick = function(){
                self.gridOptions.CurrentPage(self.gridOptions.CurrentPage() + 1);
            }
            panel.appendChild(btnNext);
            
        return panel.innerHTML;
    });
    this.notOnEdit = ko.computed(function(){
        return self.SelectedCompany() ? false : true;
    })
}

var model;
$(function(){
    model = new ViewModel();
    model.refreshGrid();
    ko.applyBindings(model);
});

function handleSortChange(){
    $('thead tr th').each(function(){
        while(this.childNodes.length > 1)
            this.removeChild(this.childNodes[1]);
        var txt = $(this).text().replace('#', 'Id');
        if(txt.length == 0) return;
        if(model.gridOptions.OrderBy().indexOf(txt) >= 0){
            var arrow = document.createElement('span');
            arrow.className = !model.gridOptions.OrderDescending() ? 'caret caret-reversed' : 'caret';
            this.appendChild(arrow);
        }
    })
}
function applyLogoUpload(){
    $("#pnlFileUpload").uploadFile({
        url: '../Company/UploadImage',
        method: "POST",
        allowedTypes: "jpg,png,gif,jpeg,bmp",
        multiple: false,
        onSuccess:function(files,data,xhr)
        {
            if(data.Status){
                model.SelectedCompany().CompanyLogo(data.Message);
                setTimeout(function(){
                    $('.ajax-file-upload-green').each(function(index){
                        if(index == 0)
                            $(this).click();
                    });
                }, 500)
            };
        },
        onError: function(files,status,errMsg)
        {       
            //files: list of files
            //status: error status
            //errMsg: error message
        }
    });
}