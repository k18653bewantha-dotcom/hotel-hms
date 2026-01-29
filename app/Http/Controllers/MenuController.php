<?php

namespace App\Http\Controllers;

use App\Models\MenuCategory;
use App\Models\MenuItem;
use Illuminate\Http\Request;

class MenuController extends Controller
{
    // Show categories & items
    public function index()
    {
        $categories = MenuCategory::with('items')->get();
        return view('menu.index', compact('categories'));
    }

    // Store category
    public function storeCategory(Request $request)
    {
        $request->validate([
            'name' => 'required'
        ]);

        MenuCategory::create($request->all());
        return back()->with('success', 'Category added');
    }

    // Store menu item
    public function storeItem(Request $request)
    {
        $request->validate([
            'menu_category_id' => 'required',
            'name' => 'required',
            'price' => 'required|numeric'
        ]);

        MenuItem::create($request->all());
        return back()->with('success', 'Item added');
    }
}
