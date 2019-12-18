function DropDownModel() {
    var self = this;
    self.projects = ko.observableArray();
    self.houses = ko.observableArray();
    self.categories = ko.observableArray();
    self.materials = ko.observableArray();
    self.uoms = ko.observableArray();
    $.getJSON('/Request/GetProjects',
      function (response) {
          self.projects(response);
      });
    $.get("/Request/GetHouses/", function (data) {
        self.houses(data);
    });

    $.get("/Request/GetCategories", function (data) {
        self.categories(data);
    });

    $.get("/Request/GetMaterials", function (data) {
        self.materials(data);
    });

    $.get("/Request/GetUOMS", function (data) {
        self.uoms(data);
    });
}

var Cart = function () {
    // Stores an array of lines, and from these, can work out the grandTotal
    var self = this;
    self.lines = ko.observableArray([new DropDownModel()]); // Put one line in by default
    // Operations
    self.addLine = function () { self.lines.push(new DropDownModel()); };
    self.removeLine = function (line) { self.lines.remove(line) };
    //self.save = function () {
    //    var dataToSave = $.map(self.lines(), function (line) {
    //        return line.product() ? {
    //            productName: line.product().name,
    //            quantity: line.quantity()
    //        } : undefined
    //    });
    //    alert("Could now send this to server: " + JSON.stringify(dataToSave));
    //};
};
//var projectModel = new DropDownModel()
ko.applyBindings(new Cart());