function CategoryModel(data, owner){
	var self = this;
	this.CategoryId = data.CategoryId;
	this.ParentCategoryId = data.ParentCategoryId;
	this.CategoryNameEs = ko.observable(data.CategoryNameEs);
	this.CategoryNameEn = ko.observable(data.CategoryNameEn);
	this.CategoryIsActive = ko.observable(data.CategoryIsActive);
	this.Childs = ko.observableArray([]);
	if(data.Childs)
		for(var i = 0; i < data.Childs.length; i++)
			self.Childs.push(new CategoryModel(data.Childs[i], self));
	this.originalData = data;

	this.setSelected = function(){
		if(model.SelectedCategory())
			model.SelectedCategory().unSelect();
		model.SelectedCategory(self);
	}
	this.saveCategory = function(){
		var data = {
			CategoryId: self.CategoryId,
			ParentCategoryId: self.ParentCategoryId,
			CategoryNameEn: self.CategoryNameEn(),
			CategoryNameEs: self.CategoryNameEs(),
			CategoryIsActive: self.CategoryIsActive()
		}
		$.ajax({
			url: '../Category/SaveCategory',
			data: data,
			type: 'POST',
			success: function(r){
				if(self.CategoryId == -1)
					owner.Childs.push(new CategoryModel(r, self));
				else
					self.originalData = r;
				model.SelectedCategory(null);
			},
			error: function(a, b, c){
				alert('Error');
			}
		})
	}
	this.addChild = function(){
		var newCat = new CategoryModel({
			CategoryId: -1,
			ParentCategoryId: self.CategoryId,
			CategoryIsActive: true
		}, self);
		self.Childs.push(newCat);
		newCat.setSelected();
	}
	this.unSelect = function(){
		self.CategoryNameEs(self.originalData.CategoryNameEs);
		self.CategoryNameEn(self.originalData.CategoryNameEn);
		self.CategoryIsActive(self.originalData.CategoryIsActive);
		if(self.CategoryId == -1)
			owner.Childs.remove(self);
		model.SelectedCategory(null);
	}

	this.notActive = ko.computed(function(){
		return !self.CategoryIsActive();
	})
	this.cssClass = ko.computed(function(){
		return self === model.SelectedCategory() ? 'bg-success' : '';
	})
	this.hasChilds = ko.computed(function(){
		return self.Childs().length > 0;
	});
	this.underEdit = ko.computed(function(){
		return model.SelectedCategory() === self;
	})
	this.notUnderEdit = ko.computed(function(){
		return model.SelectedCategory() !== self;
	})
}

function ViewModel(){
	var self  = this;
	this.Childs = ko.observableArray([]);
	this.SelectedCategory = ko.observable();

	this.init = function(){
		$.ajax({
			url: '../Category/GetCategories',
			type: 'POST',
			success: function(r){
				self.Childs.removeAll();
				for(var i = 0; i < r.length; i++)
					self.Childs.push(new CategoryModel(r[i], self));
			},
			error: function(a, b, c){

			}
		})
	}
	this.newCategory = function(){
		var newCat = new CategoryModel({
			CategoryId: -1,
			CategoryIsActive: true
		}, self);
		self.Childs.push(newCat);
		self.SelectedCategory(newCat);
	}
	this.underEdit = ko.computed(function(){
		return self.SelectedCategory() ? true : false;
	});
	this.notUnderEdit = ko.computed(function(){
		return self.SelectedCategory() ? false : true;
	})
}

$(function(){
	model = new ViewModel();
	model.init();
	ko.applyBindings(model);
})

function toggleChilds(sender){
	var p = sender.parentNode.parentNode;
	$(p).next().toggle();
	sender.className = sender.className == 'caret' ? 'caret caret-reversed' : 'caret';
}
function expandChild(sender){
	var p = sender.parentNode.parentNode;
	$(p).next().show();
}