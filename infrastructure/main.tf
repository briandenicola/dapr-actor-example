
data "azurerm_client_config" "current" {}

resource "azurerm_resource_group" "containerapp" {
  name                  = var.resource_group_name
  location              = var.location
}