function ProductModel(data){
	var self = this;
	this.ProductId = data.ProductId;
	this.CompanyId = data.CompanyId;
	this.CompanyName = data.CompanyName;
	this.CategoryId = ko.observable(data.CategoryId);
	this.CategoryName = ko.observable(data.CategoryName);
	this.ProductNameEs = ko.observable(data.ProductNameEs);
	this.ProductNameEn = ko.observable(data.ProductNameEn);
	this.ProductDescriptionEs = ko.observable(data.ProductDescriptionEs);
	this.ProductDescriptionEn = ko.observable(data.ProductDescriptionEn);
	this.ProductIsActive = ko.observable(data.ProductIsActive);
	this.ProductPrice = ko.observable(data.ProductPrice);
	this.ProductRetailPrice = ko.observable(data.ProductRetailPrice);
	this.ProductImages = ko.observableArray([]);
	this.ProductMetas = ko.observableArray([]);
	this.originalData = data;

	this.setSelected = function(){
		self.getImages();
		self.getMetas();
		model.unSelectMetas();
		$('#pnlEdit').carousel(0);
		model.SelectedProduct(self);
	}
	this.cancelEdit = function(){
		self.CategoryId(self.originalData.CategoryId);
		self.CategoryName(self.originalData.CategoryName);
		self.ProductNameEs(self.originalData.ProductNameEs);
		self.ProductNameEn(self.originalData.ProductNameEn);
		self.ProductDescriptionEs(self.originalData.ProductDescriptionEs);
		self.ProductDescriptionEn(self.originalData.ProductDescriptionEn);
		self.ProductIsActive(self.originalData.ProductIsActive);
		self.ProductPrice(self.originalData.ProductPrice);
		self.ProductRetailPrice(self.originalData.ProductRetailPrice);
		model.SelectedProduct(null);
	}
	this.saveEdit = function(){
		$.ajax({
			url: '../Product/Save',
			data: JSON.stringify(self.saveData()),
			type: 'POST',
			contentType: 'application/json',
			success: function(r){
				if(r.length > 0){
					var mn = '';
					for(var i = 0; i < r.length; i++)
						mn += r[i] + '\n';
					alert(mn);
				}
				else{
					model.SelectedProduct(null);
					model.refreshGrid();
				}
			},
			error: function(a, b, c){

			}
		})
	}
	this.getImages = function(){
		$.ajax({
			url: '../Product/GetImages',
			data: { productId: self.ProductId },
			type: 'POST',
			success: function(r){
				self.ProductImages.removeAll();
				for(var i = 0; i < r.length; i++)
					self.ProductImages.push(new ProductImageModel(r[i], self));
			},
			error: function(a, b, c){
				self.ProductImages.removeAll();
			}
		})
	}
	this.getMetas = function(){
		$.ajax({
			url: '../Product/GetMetas',
			data: { productId: self.ProductId },
			type: 'POST',
			success: function(r){
				self.ProductMetas.removeAll();
				for(var i = 0; i < r.length; i++)
					self.ProductMetas.push(new ProductMetaModel(r[i], self));
				self.selectMetas();
			},
			error: function(a, b, c){
				self.ProductMetas.removeAll();
			}
		})
	}
	this.selectMetas = function(){
		for(var i = 0; i < self.ProductMetas().length; i++)
			self.ProductMetas()[i].doSelection();
	}
	this.saveData = function(){
		var data = {
			product: {
				ProductId: self.ProductId,
				CompanyId: self.CompanyId,
				CategoryId: self.CategoryId(),
				ProductNameEs: self.ProductNameEs(),
				ProductNameEn: self.ProductNameEn(),
				ProductDescriptionEs: self.ProductDescriptionEs(),
				ProductDescriptionEn: self.ProductDescriptionEn(),
				ProductIsActive: self.ProductIsActive(),
				ProductPrice: self.ProductPrice(),
				ProductRetailPrice: self.ProductRetailPrice()
			},
			images: [],
			metas: []
		};
		for(var i = 0; i < self.ProductImages().length; i++){
			var pi = self.ProductImages()[i];
			data.images.push({
				ProductImageId: pi.ProductImageId,
				ProductId: self.ProductId,
				ProductImageUrl: pi.ProductImageUrl(),
				ProductImageThumbnail: pi.ProductImageThumbnail()
			});
		}
		var comb = combineMetas(model.getCombinationData());
		for(var i = 0; i < comb.length; i++){
			var cc = comb[i];
			data.metas.push({
				ProductId: self.ProductId,
				ProductMetaId: -1,
				ProductMetaTags: cc
			});
		}
		return data;
	}
	this.invokeInventory = function(){
		$.ajax({
			url: '../Product/GetInventory',
			data: { productId: self.ProductId },
			type: 'POST',
			success: function(r){
				model.CurrentInventory.removeAll();
				for(var i = 0; i < r.length; i++)
					model.CurrentInventory.push(new ProductInventory(r[i]));
				$('#pnlGrid').fadeOut(500, function(){
					$('#pnlInventory').fadeIn();
				});
			},
			error: function(a, b, c){

			}
		})
	}
}
function ProductImageModel(data, owner){
	var self = this;
	this.ProductImageId = data.ProductImageId;
	this.ProductId = data.ProductId;
	this.ProductImageUrl = ko.observable(data.ProductImageUrl);
	this.ProductImageThumbnail = ko.observable(data.ProductImageThumbnail);

	this.ThumbUrl = ko.computed(function(){
		return '../Content/Images/Products/' + self.ProductImageThumbnail();
	})
	this.deleteImage = function(){
		owner.ProductImages.remove(self);
	}
}
function ProductMetaModel(data, owner){
	var self = this;
	this.ProductId = data.ProductId;
	this.ProductMetaId = data.ProductMetaId;
	this.ProductMetaTags = ko.observable(data.ProductMetaTags);
    this.ProductMetaPrice = ko.observable(data.ProductMetaPrice);
    this.ProductMetaRetailPrice = ko.observable(data.ProductMetaRetailPrice);
    this.ProductMetaInventory = ko.observable(data.ProductMetaInventory);
    this.ProductMetaInventoryHold = ko.observable(data.ProductMetaInventoryHold);

    this.doSelection = function(){
    	var matches = self.ProductMetaTags().match(/\[[^\]]+\]/g);
    	for(var i = 0; i < matches.length; i++){
    		var value = matches[i].replace('[', '').replace(']', '').split('~');
    		var meta = model.getMeta(Number(value[0]));
    		if(meta){
    			meta.IsSelected(true);
    			for(var j = 0; j < meta.Options.length; j++){
    				var o = meta.Options[j];
    				if(o.MetaOptionId == value[1])
    					o.IsSelected(true);
    			}
    		}
    	}
    }
}
function ProductInventory(data){
	var self = this;
	this.ProductId = data.ProductId;
	this.ProductMetaId = data.ProductMetaId;
	this.ProductNameEs = data.ProductNameEs;
	this.ProductNameEn = data.ProductNameEn;
	this.ProductPrice = data.ProductPrice;
	this.ProductRetailPrice = data.ProductRetailPrice;
	this.ProductMetaPrice = ko.observable(data.ProductMetaPrice);
	this.ProductMetaRetailPrice = ko.observable(data.ProductMetaRetailPrice);
	this.ProductMetaTags = data.ProductMetaTags;
	this.ProductMetaTagsEn = data.ProductMetaTagsEn;
	this.ProductMetaTagsEs = data.ProductMetaTagsEs;
	this.ProductMetaInventory = ko.observable(data.ProductMetaInventory);
	this.ProductMetaInventoryHold = data.ProductMetaInventoryHold;
	this.originalData = data;

	this.Version = '';
	var matches = this.ProductMetaTagsEn.match(/\[[^\]]+\]/g);
	for(var i = 0; i < matches.length; i++){
		var a = matches[i].replace('[', '').replace(']', '').split('~');
		this.Version += a[1] + ' ,';
	}

	this.toBusiness = function(){
		return {
			ProductMetaId: self.ProductMetaId,
			ProductMetaInventory: self.ProductMetaInventory(),
			ProductMetaPrice: self.ProductMetaPrice(),
			ProductMetaRetailPrice: self.ProductMetaRetailPrice()
		}
	}
}
function MetaModel(data){
	var self = this;
	this.MetaId = data.MetaId;
	this.MetaNameEs = data.MetaNameEs;
	this.MetaNameEn = data.MetaNameEn;
	this.Options = [];
	if(data.Options)
		for(var i = 0; i < data.Options.length; i++)
			this.Options.push(new MetaOptionModel(data.Options[i], self));
	this.IsSelected  = ko.observable(false);

	this.unSelect = function(){
		self.IsSelected(false);
		for(var i = 0; i < self.Options.length; i++)
			self.Options[i].IsSelected(false);
	}
}
function MetaOptionModel(data, owner){
	var self = this;
	this.MetaOptionId = data.MetaOptionId;
	this.MetaId = data.MetaId;
	this.MetaOptionNameEs = data.MetaOptionNameEs;
	this.MetaOptionNameEn = data.MetaOptionNameEn;
	this.MetaOptionValue = data.MetaOptionValue;
	this.IsSelected = ko.observable(false);

	this.toBusiness = function(){
		return "[" + owner.MetaId + "~" + self.MetaOptionId + "]";
	}
}

function ViewModel(){
	var self = this;
	this.Products = ko.observableArray([]);
	this.SelectedProduct = ko.observable();
	this.CurrentInventory = ko.observableArray([]);
	this.Categories = ko.observableArray([]);
	this.Categories2 = ko.observableArray([]);
	this.Metas = ko.observableArray([]);
	this.SUPRESS = false;
	this.LAST_SORT_INDEX = 0;

	this.gridOptions = {
        RowsPerPage: ko.observable(25),
        CurrentPage: ko.observable(0),
        OrderBy: ko.observable('CompanyId'),
        OrderDescending: ko.observable(false),
        TotalCount: ko.observable(0),
        PageSizes: [5, 10, 25, 50],
        TotalPages: ko.observable(0)
    }
	this.filter = {
		UsePrice: ko.observable(false),
		Like: ko.observable(''),
		CategoryId: ko.observable(-1),
		Price: ko.observable(0),
		Comparisson: ko.observable(0),
		ComparissonText: ko.observable("<=")
	}

	this.getCategories = function(){
		this.SUPRESS = true;
		$.ajax({
			url: '../Category/GetCategoriesList',
			type: 'POST',
			success: function(r){
				self.Categories.removeAll();
				self.Categories2.removeAll();
				self.Categories.push({ CategoryNameEs: '*', CategoryNameEn: '*', CategoryId: -1 });
				for(var i = 0; i < r.length; i++){
					self.Categories.push({ CategoryNameEs: r[i].CategoryNameEs, CategoryNameEn: r[i].CategoryNameEn, CategoryId: r[i].CategoryId });
					self.Categories2.push({ CategoryNameEs: r[i].CategoryNameEs, CategoryNameEn: r[i].CategoryNameEn, CategoryId: r[i].CategoryId });
				}
				self.SUPRESS = false;
			}
		});
	}
	this.getMetas = function(){
		$.ajax({
			url: '../Meta/GetAll',
			type: 'POST',
			success: function(r){
				self.Metas.removeAll();
				for(var i = 0; i < r.length; i++)
					self.Metas.push(new MetaModel(r[i], self));
			},
			error: function(){

			}
		})
	}
	this.getCount = function(){
		$.ajax({
			url: '../Product/GetCount',
			type: 'POST',
			data: self.filterValues(),
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
            url: '../Product/Get',
            data: JSON.stringify({
                options: self.gridOptionsValues(),
                filter: self.filterValues()
            }),
            contentType: 'application/json',
            type: 'POST',
            success: function(r){
                self.Products.removeAll();
                for(var i = 0; i < r.length; i++)
                    self.Products.push(new ProductModel(r[i]));
            },
            error: function(a, b, c){

            }
        })
    }
    this.setSortMode = function(orderBy, index){
        if(self.gridOptions.OrderBy() == orderBy){
            self.gridOptions.OrderDescending(!self.gridOptions.OrderDescending())
        }
        else{
            self.gridOptions.OrderBy(orderBy);
            self.gridOptions.OrderDescending(false);
        }
        self.getElements();
        handleSortChange(index);
        self.LAST_SORT_INDEX = index;
    }
    this.refreshGrid = function(){
        self.getCount();
        self.getElements();
        handleSortChange(self.LAST_SORT_INDEX);
    }
	this.init = function(){
		self.getCategories();
		self.getMetas();
		self.refreshGrid();
	}
	this.filterValues = function(){
		var ret = {
			Like: self.filter.Like(),
			Category: self.filter.CategoryId()
		}
		if(self.filter.UsePrice()){
			ret.Price = self.filter.Price();
			var c = self.filter.Comparisson()
			ret.PriceComparisson = c == 0 ? 'LowerThan' :
				c == 1 ? 'LowerOrEqualTo' :
				c == 2 ? 'EqualTo' :
				c == 3 ? 'DifferentOf' : 
				c == 4 ? 'GreaterOrEqualTo' : 
				'GreaterThan';
		}
		return ret;
	}
	this.gridOptionsValues = function(){
		return {
			RowsPerPage: self.gridOptions.RowsPerPage(),
			CurrentPage: self.gridOptions.CurrentPage(),
			OrderBy: self.gridOptions.OrderBy(),
			OrderDescending: self.gridOptions.OrderDescending()
		}
	}
	this.setComparisson = function(v){
		self.filter.Comparisson(v);
		switch(v){
			case 0: self.filter.ComparissonText('<'); break;
			case 1: self.filter.ComparissonText('<='); break;
			case 2: self.filter.ComparissonText('=='); break;
			case 3: self.filter.ComparissonText('!='); break;
			case 4: self.filter.ComparissonText('>='); break;
			case 5: self.filter.ComparissonText('>'); break;
		}
	}
	this.newProduct = function(){
		self.SelectedProduct(new ProductModel({
			ProductId: -1,
			ProductIsActive: true
		}))
		self.unSelectMetas();
		$('#pnlEdit').carousel(0);
	}
	this.unSelectMetas = function(){
		for(var i = 0; i < self.Metas().length; i++)
			self.Metas()[i].unSelect();
	}
	this.getMeta = function(v){
		for(var i = 0; i < self.Metas().length; i++)
			if(self.Metas()[i].MetaId == v)
				return self.Metas()[i];
		return null;
	}
	this.getCombinationData = function(){
		var perm =  [];
		var mdat = self.SelectedMetas();
		for(var i = 0; i < mdat.length; i++){
			var pcurrent = [];
			for(var j = 0; j < mdat[i].Options.length; j++)
				if(mdat[i].Options[j].IsSelected())
					pcurrent.push(mdat[i].Options[j].toBusiness());
			if(pcurrent.length > 0)
				perm.push(pcurrent);
		}
		return perm;
	}
	this.saveInventory = function(){
		var metaList = [];
		for(var i = 0; i < self.CurrentInventory().length; i++)
			metaList.push(self.CurrentInventory()[i].toBusiness());
		$.ajax({
			url: '../Product/SaveInventory',
			data: JSON.stringify(metaList),
			type: 'POST',
			contentType: 'application/json',
			success: self.endInventory,
			error: self.endInventory
		})
	}
	this.endInventory = function(){
		$('#pnlInventory').fadeOut(500, function(){
			$('#pnlGrid').fadeIn(500, function(){
				self.CurrentInventory.removeAll();
			});
		});
	}
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
	this.filter.Like.subscribe(function(){
        var c = self.gridOptions.CurrentPage();
        if(c == 0)
            self.refreshGrid();
        else
            self.gridOptions.CurrentPage(0);
    });
    this.filter.CategoryId.subscribe(function(){
    	if(self.SUPRESS)
    		return;
        var c = self.gridOptions.CurrentPage();
        if(c == 0)
            self.refreshGrid();
        else
            self.gridOptions.CurrentPage(0);
    });
    this.filter.Price.subscribe(function(){
        var c = self.gridOptions.CurrentPage();
        if(c == 0)
            self.refreshGrid();
        else
            self.gridOptions.CurrentPage(0);
    });
    this.filter.UsePrice.subscribe(function(){
        var c = self.gridOptions.CurrentPage();
        if(c == 0)
            self.refreshGrid();
        else
            self.gridOptions.CurrentPage(0);
    });
    this.filter.Comparisson.subscribe(function(){
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
    this.onSelection = ko.computed(function(){
    	return self.SelectedProduct() ? false : true;
    });
    this.SelectedMetas = ko.computed(function(){
    	var arr = [];
    	for(var i = 0; i < self.Metas().length; i++)
    		if(self.Metas()[i].IsSelected())
    			arr.push(self.Metas()[i]);
    	return arr;
    });
}

var model;
$(function(){
	model = new ViewModel();
	model.init();
	ko.applyBindings(model);

	$('#pnlEdit').carousel({
		pause: true,
		interval: false
	});

	$("#pnlFileUpload").uploadFile({
        url: '../Product/UploadImage',
        method: "POST",
        allowedTypes: "jpg,png,gif,jpeg,bmp",
        multiple: false,
        onSuccess:function(files,data,xhr)
        {
            if(data.Status){
            	//Push to SelectedProductImages
                //model.SelectedCompany().CompanyLogo(data.Message);
                model.SelectedProduct().ProductImages.push(new ProductImageModel({
                	ProductImageId: -1,
                	ProductId: model.SelectedProduct().ProductId,
                	ProductImageUrl: data.Message,
                	ProductImageThumbnail: 'thumb_' + data.Message
                }, model.SelectedProduct()))
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
})

function handleSortChange(sortIndex){
    $('thead tr th').each(function(index){
        while(this.childNodes.length > 1)
            this.removeChild(this.childNodes[1]);
        if(index != sortIndex) return;
        var arrow = document.createElement('span');
        arrow.className = !model.gridOptions.OrderDescending() ? 'caret caret-reversed' : 'caret';
        this.appendChild(arrow);
    })
}

function combineMetas(arr) {
	if (arr.length === 0)
		return [];
	else if (arr.length ===1)
		return arr[0]
	else{
		var result = [];
		var rest = combineMetas(arr.slice(1));
		for (var c in rest)
			for (var i = 0; i < arr[0].length; i++)
				result.push(arr[0][i] + rest[c]);
		return result;
	}
}