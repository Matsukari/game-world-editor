
-- C# lsp setup
-- Do not open another project solution
vim.cmd [[
    set runtimepath+=~/.vim/pack/dist/opt/*
    set packpath+=~/.vim/pack/dist/opt/*
]]
local nvim_lsp = require('lspconfig')

nvim_lsp.omnisharp.setup{
    on_attach = function (client)
      vim.api.nvim_exec([[
          autocmd VimEnter * echo "Omnisharp ok"
        ]], false)
    end,
    cmd = { '../../deps/omnisharp/OmniSharp', '--languageserver' , '--hostPID', tostring(vim.fn.getpid()) };
}

-- require("symbols-outline").setup()
-- Enable diagnostics update in insert mode
vim.api.nvim_exec([[
  augroup DiagnosticsConfig
    autocmd!
    autocmd InsertEnter * lua vim.diagnostic.config({ update_in_insert = true })
    autocmd InsertLeave * lua vim.diagnostic.config({ update_in_insert = false })
  augroup END
]], false)


vim.api.nvim_set_keymap('n', '<leader>rn', '<cmd>lua vim.lsp.buf.rename()<CR>', { noremap = true, silent = true })
vim.api.nvim_set_keymap('n', '<leader>ca', '<cmd>lua vim.lsp.buf.code_action()<CR>', { noremap = true, silent = true })


-- vim.cmd('PlugInstall')
-- vim.cmd('close')
-- vim.cmd('edit Source/TestGame.Core/TestGameProcess.cs')
-- vim.cmd('TagbarOpenAutoClose')

-- https://medium.com/@shaikzahid0713/git-integration-in-neovim-a6f26c424b58
--
--

