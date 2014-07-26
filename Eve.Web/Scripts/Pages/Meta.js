function MetaModel(data){
	var self = this;
	this.MetaId = data.MetaId;
	this.MetaNameEs = ko.observable(data.MetaNameEs);
	this.MetaNameEn = ko.observable(data.MetaNameEn);
	this.Options = ko.observableArray([]);
	this.originalData = data;
	if(data.Options)
		for(var i = 0; i < data.Options.length; i++)
			this.Options.push(new MetaOptionModel(data.Options[i], self));

	this.beginEdit = function(){
        model.SelectedMeta(self);
    }
    this.cancelEdit = function(){
    	self.MetaNameEn(self.originalData.MetaNameEn);
    	self.MetaNameEs(self.originalData.MetaNameEs);
    	self.Options.removeAll();
    	if(self.originalData.Options)
			for(var i = 0; i < self.originalData.Options.length; i++)
				this.Options.push(new MetaOptionModel(self.originalData.Options[i], self));
        model.SelectedMeta(null);
    }
    this.saveEdit = function(){
    	var data = {
    		MetaId: self.MetaId,
    		MetaNameEs: self.MetaNameEs(),
    		MetaNameEn: self.MetaNameEn(),
    		Options: []
    	}
    	for(var i = 0; i < self.Options().length; i++)
    		data.Options.push(self.Options()[i].toBusiness());

        $.ajax({
            url: '../Meta/Save',
            type: 'POST',
            data: JSON.stringify(data),
            contentType: 'application/json',
            success: function(r){
            	console.log(r);
                model.SelectedMeta(null);
                model.refreshGrid();
            },
            error: function(a, b, c){
            	console.log(a)
            }
        })
    }
    this.addOption = function(){
    	self.Options.push(new MetaOptionModel({
    		MetaId: self.MetaId,
    		MetaOptionId: -1,
    		MetaOptionNameEs: '',
    		MetaOptionNameEn: '',
    		MetaOptionValue: '',
    	}, self));
    }
}

function MetaOptionModel(data, owner){
	var self = this;
	this.MetaOptionId = data.MetaOptionId;
	this.MetaId = data.MetaId;
	this.MetaOptionNameEs = ko.observable(data.MetaOptionNameEs);
	this.MetaOptionNameEn = ko.observable(data.MetaOptionNameEn);
	this.MetaOptionValue = ko.observable(data.MetaOptionValue);
	this.originalData = data;

	this.toBusiness = function(){
		return {
			MetaOptionId: self.MetaOptionId,
			MetaId: self.MetaId,
			MetaOptionNameEn: self.MetaOptionNameEn(),
			MetaOptionNameEs: self.MetaOptionNameEs(),
			MetaOptionValue: self.MetaOptionValue(),
		};
	}
	this.deleteOption = function(){
		owner.Options.remove(self);
	}
	this.domValue = ko.computed(function(){
		return displayForValue(self.MetaOptionValue() || self.MetaOptionNameEn() );
	})
}

function ViewModel(){
	var self = this;
	this.Metas = ko.observableArray([]);
    this.SelectedMeta = ko.observable();
    this.gridOptions = {
        RowsPerPage: ko.observable(25),
        CurrentPage: ko.observable(0),
        OrderBy: ko.observable('MetaId'),
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
            url: '../Meta/GetCount',
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
            url: '../Meta/Get',
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
                self.Metas.removeAll();
                for(var i = 0; i < r.length; i++)
                    self.Metas.push(new MetaModel(r[i]));
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
    this.newMeta = function(){
        self.SelectedMeta(new MetaModel({
            MetaId: -1,
            MetaNameEs: '',
            MetaNameEn: ''
        }));
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
        return self.SelectedMeta() ? false : true;
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
function displayForValue(value){
    if(!value)
        return "<span>" + value + "</span>";
	var v = value.replace(/ /g, '');
	var hex  = /([a-f0-9]{3})|([a-f0-9]{6})/gi;
	var rgb = /rgb\([0-9]{1,3}\,[0-9]{1,3}\,[0-9]{1,3}\)/gi;
	var rgba = /rgb\([0-9]{1,3}\,[0-9]{1,3}\,[0-9]{1,3},[0-1]?(\.[0-9]+)?\)/gi;
	if(hex.test(v) || rgb.test(v) || rgba.test(v))
		return "<span class='color-pick' style='background-color: " + v + ";'>&nbsp;</span>";
	return "<span>" + value + "</span>";
}