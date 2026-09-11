using AppMinhasCompras.Models;

namespace AppMinhasCompras.Views;

public partial class NovoProduto : ContentPage
{
	public NovoProduto()
	{
		InitializeComponent();
	}

    private async void ToolbarItem_Clicked(object sender, EventArgs e)
    {
		try
		{
			string categoriaFinal = txt_nova_categoria.Text;

			if (!string.IsNullOrEmpty(txt_nova_categoria.Text))
			{
				categoriaFinal = txt_nova_categoria.Text.Trim();
            }

			else if (pck_categoria_cadastro.SelectedItem != null)
			{
				categoriaFinal = pck_categoria_cadastro.SelectedItem.ToString();
            }

            if (!string.IsNullOrEmpty(categoriaFinal))
            {
                categoriaFinal = char.ToUpper(categoriaFinal[0]) + categoriaFinal.Substring(1).ToLower();
            }

            Produto p = new Produto
			{
				Descricao = txt_descricao.Text,
				Quantidade = Convert.ToDouble(txt_quantidade.Text),
				Categoria = categoriaFinal,
                Preco = Convert.ToDouble(txt_preco.Text)
			};

			await App.Db.Insert(p);
			await DisplayAlert("Sucesso!", "Registro Inserido", "OK");
			await Navigation.PopAsync();

		} catch (Exception ex)
		{
			DisplayAlert("Ops", ex.Message, "OK");
		}
    }

    private async void CargarCategoriasExistentes()
    {
        List<Produto> todos = await App.Db.GetAll();

        // Pega categorias únicas, sem diferenciar maiúsculas/minúsculas
        var categorias = todos
            .Where(p => !string.IsNullOrEmpty(p.Categoria))
            .Select(p => char.ToUpper(p.Categoria[0]) + p.Categoria.Substring(1).ToLower())
            .Distinct()
            .ToList();

        pck_categoria_cadastro.ItemsSource = categorias;
    }

	protected override void OnAppearing()
    {
        base.OnAppearing();
        CargarCategoriasExistentes();
    }
}