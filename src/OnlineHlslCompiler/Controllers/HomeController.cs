﻿using System;
using System.Collections.Generic;
using System.Web.Mvc;
using OnlineHlslCompiler.Framework;
using OnlineHlslCompiler.Framework.Dxc;
using OnlineHlslCompiler.Framework.Fxc;
using OnlineHlslCompiler.ViewModels;

namespace OnlineHlslCompiler.Controllers
{
    public class HomeController : Controller
    {
        private static readonly Dictionary<Compiler, IShaderCompiler> Compilers = new Dictionary<Compiler, IShaderCompiler>
        {
            { Compiler.NewCompiler, new DxcCompiler() },
            { Compiler.OldCompiler, new FxcCompiler() }
        };

        public ActionResult Index()
        {
            return View(new HomeViewModel
            {
                Code = @"float4 PS() : SV_Target
{
    return float4(1, 0, 0, 1);
}",
                Compiler = Compiler.NewCompiler,
                TargetProfile = TargetProfile.ps_6_0,
                EntryPointName = "PS"
            });
        }

        [HttpPost]
        public ActionResult Compile(HomeViewModel model)
        {
            try
            {
                var compiler = Compilers[model.Compiler];

                var compilationResult = compiler.Compile(
                    model.Code, model.EntryPointName,
                    model.TargetProfile.ToString());

                return Json(new CompilationResultViewModel
                {
                    HasErrors = compilationResult.HasErrors,
                    Message = compilationResult.Message,
                    Disassembly = compilationResult.Disassembly,
                    DisassemblyFormat = model.Compiler.ToString()
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    HasErrors = true,
                    Message = ex.ToString()
                });
            }
        }
    }
}